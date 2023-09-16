using Core.Common.Model;
using Core.Common.Model.IdentityModels.Identity;
using Core.Extensions;
using Core.Interfaces;
using Core.Common.Settings;
using Domain.Entities.Identity;
using HRShared.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static Core.Common.Model.IdentityModels.Identity.LoginRequestModel;

namespace Infrastructure.Implementation
{
    public class TokenService : ITokenService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserRoleService _userRoleService;
        private readonly IRoleClaimsService _roleClaimsService;
        private readonly JwtSettings _jwtSettings;

        public TokenService(UserManager<ApplicationUser> userManager, IOptions<JwtSettings> jwtSettings, IUserRoleService userRoleService, IRoleClaimsService roleClaimsService)
        {
            _userManager = userManager;
            _jwtSettings = jwtSettings.Value;
            _roleClaimsService = roleClaimsService;
            _userRoleService = userRoleService;
        }




        public async Task<ResponseModel<LoginResponse>> GetTokenAsync(LoginModel request, string ipAddress, CancellationToken cancellationToken)
        {


            var user = await _userManager.FindByEmailAsync(request.Email.Trim().Normalize());
            if (user is null)
            {
                return ResponseModel<LoginResponse>.Failure("Invalid email and password");

            }

            if (!user.IsActive)
            {
                return ResponseModel<LoginResponse>.Failure("Invalid records");
            }

            if (!user.EmailConfirmed)
            {
                return ResponseModel<LoginResponse>.Failure("Email Awaiting confirmation");
            }

            if (!await _userManager.CheckPasswordAsync(user, request.Password))
            {
                return ResponseModel<LoginResponse>.Failure("Invalid email and pasword");

            }

            //get the user role,
            var userRole = await _userRoleService.GetByUserAsync(Guid.Parse(user.Id));

            if (!userRole.IsSuccessful || userRole.Data == null)
            {
                return ResponseModel<LoginResponse>.Failure("Invalid role record");

            }



            //var roleClaims = await _roleClaimsService.GetRoleClaimsListAsync(userRole.Data.RoleId);


            var userClaims = new List<string?>();

            // if(roleClaims.Data != null)
            // {
            //     userClaims = roleClaims.Data.Select(x => x.Claims?.ClaimName).ToList();
            // }

            //get the company plan claims

            var loginReponse = await GenerateTokensAndUpdateUser(user, userClaims, userRole.Data?.RoleModel?.Name);

            return ResponseModel<LoginResponse>.Success(loginReponse);
        }

        public async Task<ResponseModel<LoginResponse>> RefreshTokenAsync(RefreshTokenModel request, string ipAddress)
        {
            var userPrincipal = GetPrincipalFromExpiredToken(request.Token);
            string? userEmail = userPrincipal.GetEmail();
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user is null)
            {
                return ResponseModel<LoginResponse>.Failure("authentication failed");
            }

            if (user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return ResponseModel<LoginResponse>.Failure("invalid referesh token");
            }

            string? permissions = userPrincipal.GetPermissions();


            var roleClaims = permissions is not null ? JsonConvert.DeserializeObject<List<string?>>(permissions) : new List<string?>();


            var loginResponse = await GenerateTokensAndUpdateUser(user, roleClaims, ipAddress);

            return ResponseModel<LoginResponse>.Success(loginResponse);

        }

        private async Task<LoginResponse> GenerateTokensAndUpdateUser(ApplicationUser user, List<string?> roleClaims, string role)
        {
            string token = GenerateJwt(user, roleClaims, role);

            user.RefreshToken = GenerateRefreshToken();
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationInDays);

            await _userManager.UpdateAsync(user);

            return new LoginResponse(token, user.RefreshToken, user.RefreshTokenExpiryTime, user.CompanyId.ToString(), user.Id);
        }

        private string GenerateJwt(ApplicationUser user, List<string?> roleClaims, string role)
        {

            var authClaims = GetClaims(user, roleClaims, role);

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.ValidIssuer,
                audience: _jwtSettings.ValidAudience,
                expires: DateTime.Now.AddYears(2),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            // LoginResponse loginResponse = new LoginResponse
            // {
            //     Token = new JwtSecurityTokenHandler().WriteToken(token),
            //     ExpiryDate = token.ValidTo
            //
            //
            // };

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);
            // return GenerateEncryptedToken(GetSigningCredentials(), GetClaims(user, roleClaims, role));

        }

        private IEnumerable<Claim> GetClaims(ApplicationUser user, List<string?> roleClaims, string role)
        {
            //get the user role
            //get the user
            return new List<Claim>
            {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email),
            new(TenantClaimConstants.Fullname, $"{user.FirstName} {user.LastName}"),
            new(ClaimTypes.Sid, user.CompanyId.ToString()),
            new(ClaimTypes.Name, user.FirstName ?? string.Empty),
            new(ClaimTypes.Role, role ?? string.Empty),
            new(ClaimTypes.Surname, user.LastName ?? string.Empty),
            new(TenantClaimConstants.Company, user.CompanyId.ToString()),
            //new(TenantClaimConstants.Tenant, _currentTenant!.Id),
            new(TenantClaimConstants.ImageUrl, user.ImageUrl ?? string.Empty),
            new(ClaimTypes.MobilePhone, user.PhoneNumber ?? string.Empty),
           // new Claim(TenantClaimConstants.Permission, JsonConvert.SerializeObject(roleClaims))
            };
        }


        private string GenerateRefreshToken()
        {
            byte[] randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private string GenerateEncryptedToken(SigningCredentials signingCredentials, IEnumerable<Claim> claims)
        {
            var token = new JwtSecurityToken(
               claims: claims,
               expires: DateTime.UtcNow.AddMinutes(_jwtSettings.TokenExpirationInMinutes),
               signingCredentials: signingCredentials);
            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            if (string.IsNullOrEmpty(_jwtSettings.Key))
            {
                throw new InvalidOperationException("No Key defined in JwtSettings config.");
            }

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key)),
                ValidateIssuer = false,
                ValidateAudience = false,
                RoleClaimType = ClaimTypes.Role,
                ClockSkew = TimeSpan.Zero,
                ValidateLifetime = false
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(
                    SecurityAlgorithms.HmacSha256,
                    StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception("identity.invalidtoken");
            }

            return principal;
        }

        private SigningCredentials GetSigningCredentials()
        {
            if (string.IsNullOrEmpty(_jwtSettings.Key))
            {
                throw new InvalidOperationException("No Key defined in JwtSettings config.");
            }

            byte[] secret = Encoding.UTF8.GetBytes(_jwtSettings.Key);
            return new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256);
        }
    }
}
