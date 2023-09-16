using AutoMapper;
using Core.Common.Model;
using Core.Common.Model.IdentityModels.Identity;
using Core.Common.Settings;
using Core.Extensions;
using Core.Interfaces;
using Domain.Entities.Identity;
using HRShared.Common;
using HRShared.Helpers;
using Infrastructure.Persistence.Context;
using Infrastructure.Providers.Interface;
using Infrastructure.Providers.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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

namespace Infrastructure.Implementation
{
    public class IdentityService : IIdentityService
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ILogger<IdentityService> _logger;
        private readonly IMapper _mapper;
        private readonly IMailService _mailService;
        private readonly EmailSettings _mailSettings;
        private readonly JwtSettings _jwtSettings;
        private readonly ApplicationDbContext _dbContext;

        public IdentityService(SignInManager<ApplicationUser> signInManager,
            ILogger<IdentityService> logger,
            IOptions<JwtSettings> jwtSettings,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            IMapper mapper,
            IOptions<EmailSettings> emailSettings,
            IMailService mailService,
            ApplicationDbContext context)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
            _logger = logger;
            _mailSettings = emailSettings.Value;
            _mailService = mailService;
            _dbContext = context;
        }

        public async Task<string> RegisterAsync(RegisterUserModel request, string origin)
        {
            var user = new ApplicationUser
            {
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                UserName = request.UserName,
                PhoneNumber = request.PhoneNumber,
                CompanyId = request.CompanyId,
                IsActive = true
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                return "Error Occured";
                // throw new InternalServerException("Validation Errors Occurred.", result.Errors.Select(a => a.Description.ToString()).ToList());
            }

            await _userManager.AddToRoleAsync(user, ApplicationRoleConstant.Admin);

            var messages = new List<string> { string.Format("User {0} Registered.", user.UserName) };

            return string.Join(Environment.NewLine, messages);
        }

        public async Task<string> NewRegisterAsync(string firstName, string lastName, string Email, string Username,
            string password, string confirmpassword, string phonenumber, string CompanyId, string origin)
        {
            //first check if the user exist before

            var userExists = await _userManager.FindByEmailAsync(Email);

            if (userExists != null)
                return userExists.Id;

            var user = new ApplicationUser
            {
                Email = Email,
                FirstName = firstName,
                LastName = lastName,
                UserName = Username,
                PhoneNumber = phonenumber,
                CompanyId = Guid.Parse(CompanyId),
                IsActive = true
            };


            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                return "Error Occured";
            }
            await _userManager.AddToRoleAsync(user, ApplicationRoleConstant.Admin);

            var messages = new List<string> { string.Format("User {0} Registered.", user.UserName) };

            //get company details and send email
            var userDetails = await GetSingleByEmail(user.Email);
            var companyDetails = await _dbContext.Companies.Where(x => x.Id == user.CompanyId).FirstOrDefaultAsync();

            await SendAccountCreationMail(userDetails.Data, companyDetails.NameOfOrganization, password);

            return user.Id;
        }
        public async Task<string> NewJobApplicantRegisterAsync(string firstName, string lastName, string Email, string Username,
            string password, string confirmpassword, string phonenumber, string CompanyId, string origin)
        {

            //parse the company Id before use
            var companyId = Guid.Parse(CompanyId);

            //first check if the user exist before
            var userExists = await _userManager.FindByEmailAsync(Email);

            if (userExists != null && userExists.CompanyId == companyId)
                return userExists.Id;

            var user = new ApplicationUser
            {
                Email = Email,
                FirstName = firstName,
                LastName = lastName,
                UserName = Username,
                PhoneNumber = phonenumber,
                CompanyId = Guid.Parse(CompanyId),
                IsActive = true
            };

            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                return "Error Occured";
                // throw new InternalServerException("Validation Errors Occurred.", result.Errors.Select(a => a.Description.ToString()).ToList());
            }


            // await _roleManager.CreateAsync(new ApplicationRole(ApplicationRoleConstant.Applicant));
            await _userManager.AddToRoleAsync(user, ApplicationRoleConstant.Applicant);

            //get company details and send email
            var userDetails = await GetSingleByEmail(user.Email);
            var companyDetails = await _dbContext.Companies.Where(x => x.Id == user.CompanyId).FirstOrDefaultAsync();


            await SendApplicantAccountCreationMail(userDetails.Data, companyDetails.NameOfOrganization, password);

            return user.Id;
        }


        public async Task<ResponseModel<bool>> ChangePasswordAsync(ChangePasswordModel model, Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
            {
                return ResponseModel<bool>.Failure("User not found");
            }


            var identityResult = await _userManager.ChangePasswordAsync(user, model.Password, model.NewPassword);

            if (!identityResult.Succeeded)
            {
                var response = new ResponseModel<bool>();
                response.Errors = identityResult.Errors.Select(x => x.Description).ToList();
                response.Message = "An error occured!";
                response.IsSuccessful = false;
                return response;
            }
            return ResponseModel<bool>.Success(true, "Password changed successfully");
        }


        public Task<string> ConfirmPhoneNumberAsync(Guid userId, string code)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseModel<string>> ForgotPasswordAsync(UserResponseModel userModel, string origin, string companyName)
        {

            if (string.IsNullOrWhiteSpace(origin))
            {
                origin = _mailSettings.VerificationBaseUrl;
            }

            var user = await _userManager.Users
                .Where(u => u.Email == userModel.Email)
                .FirstOrDefaultAsync();

            if (user is null)
            {
                return ResponseModel<string>.Failure("An Error occured");
            }

            if (user is null)
            {
                return ResponseModel<string>.Failure("An Error occured");
            }

            string code = await _userManager.GeneratePasswordResetTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            const string route = "auth/passwordreset";
            var endpointUri = new Uri(string.Concat($"{origin}/", route));
            string passwordResetUrl = QueryHelpers.AddQueryString(endpointUri.ToString(), "Token", code);
            passwordResetUrl = QueryHelpers.AddQueryString(passwordResetUrl, "email", user.Email);

            string buildContent = $"Hi {user.FirstName}, " +
                $"<p>There was a request to change your password</p>" +
                $"<br>" +
                $"<p>If you did not make this request, kindly ignore this email, otherwise click the button below to reset password</p>" +
                $"<div class = 'row'>" +
                $"<div class = 'col-md-12 col-sm-12'>" +
                $"<a href = '{passwordResetUrl}'>" +
                $"<button class = 'btn btn-primary'>Reset password</button>" +
                $"</a>" +
                $"</div>" +
                $"</div>";
            var mailRequest = new MailRequest(
                new List<string> { user.Email },
                "Reset Password", MailTemplateHelper.GenerateMailContent(buildContent, "base-layout.html", companyName));

            await _mailService.SendAsync(mailRequest);

            return ResponseModel<string>.Success("Password Reset Mail has been sent to your authorized Email.");
        }

        //build the Email content
        //call the template handle to replace [[content]] with built content
        //send mail

        public Task<string> GetOrCreateFromPrincipalAsync(ClaimsPrincipal principal)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseModel<UserResponseModel>> GetSingle(Guid userId)
        {
            try
            {
                var user = await _userManager.Users
            .Where(u => u.Id == userId.ToString())
            .FirstOrDefaultAsync();
                return ResponseModel<UserResponseModel>.Success(_mapper.Map<UserResponseModel>(user));
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception occured while getting user: {ex.Message}", nameof(GetSingle));
                return ResponseModel<UserResponseModel>.Failure("Exception error");
            }
        }

        public async Task<ResponseModel<UserResponseModel>> GetSingleByEmail(string email)
        {
            try
            {
                var user = await _userManager.Users
            .Where(u => u.Email == email)
            .FirstOrDefaultAsync();
                return ResponseModel<UserResponseModel>.Success(_mapper.Map<UserResponseModel>(user));
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception occured while getting user: {ex.Message}", nameof(GetSingle));
                return ResponseModel<UserResponseModel>.Failure("Exception error");
            }
        }

      
        public async Task<ResponseModel<string>> ResetPasswordAsync(ResetPasswordModel request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email?.Normalize());

            if (user == null)
            {
                return ResponseModel<string>.Failure("An Error has occurred!");
            }
            var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Token));
            var result = await _userManager.ResetPasswordAsync(user, code, request.Password);

            if (!result.Succeeded)
            {
                return ResponseModel<string>.Failure("An Error has occurred! Invalid token");

            }
            return ResponseModel<string>.Success("Password Reset Successful");


        }

        private async Task<string> GetEmailVerificationUriAsync(ApplicationUser user, string origin)
        {
            if (string.IsNullOrWhiteSpace(origin))
            {
                origin = _mailSettings.VerificationBaseUrl;
            }

            string code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            const string route = "auth/emailConfirmation";
            var endpointUri = new Uri(string.Concat($"{origin}/", route));
            string verificationUri = QueryHelpers.AddQueryString(endpointUri.ToString(), "userid", user.Id);
            verificationUri = QueryHelpers.AddQueryString(verificationUri, "code", code);
            //verificationUri = QueryHelpers.AddQueryString(verificationUri, MultitenancyConstants.TenantIdName, _currentTenant.Id);
            return verificationUri;
        }

        public async Task<ResponseModel<bool>> ConfirmEmailAsync(Guid userId, string code, CancellationToken cancellationToken)
        {
            var user = await _userManager.Users
                .Where(u => u.Id == userId.ToString())
                .FirstOrDefaultAsync(cancellationToken);

            if (user == null)
                return ResponseModel<bool>.Failure("Error confirming email, email record not found");

            if (user.EmailConfirmed)
            {
                return ResponseModel<bool>.Success(true, "Email record confirmed");
            }

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
            {
                user.EmailConfirmed = true;
                await _userManager.UpdateAsync(user);
            }

            user.EmailConfirmed = true;
            await _userManager.UpdateAsync(user);

            // return result.Succeeded ? ResponseModel<bool>.Success(true, $"Account Confirmed for E-Mail {user.Email}. You can now use the /api/auth/login endpoint to generate JWT.")
            //     : ResponseModel<bool>.Failure("Error occurred while confirming E-mail");


            return user.EmailConfirmed ? ResponseModel<bool>.Success(true, $"Account Confirmed for E-Mail {user.Email}. You can now use the /api/auth/login endpoint to generate JWT.")
                : ResponseModel<bool>.Failure("Error occurred while confirming E-mail");
        }
        public async Task<ResponseModel<bool>> ConfirmOtpAsync(string Email, string code)
        {
            var user = await _userManager.Users
                .Where(u => u.Email == Email)
                .FirstOrDefaultAsync();

            if (user == null)
                return ResponseModel<bool>.Failure("Error confirming user, User record not found");

            if (user.EmailConfirmed)
            {
                return ResponseModel<bool>.Success(true, "User record confirmed");
            }

            if (user.EmailConfirmed = true)
            {
               
                await _userManager.UpdateAsync(user);
            }

            return user.EmailConfirmed ? ResponseModel<bool>.Success(true, $"Account Confirmed for OTP/E-Mail {user.Email}. You can now use the /api/auth/login endpoint to generate JWT.")
                : ResponseModel<bool>.Failure("Error occurred while confirming OTP/E-mail");
        }

        public Task UpdateProfileAsync(UpdateUserModel request, Guid userId)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseModel<string>> SendActivationMail(UserResponseModel userModel, string companyName)
        {
            try
            {
                var user = await _userManager.Users
                .Where(u => u.Email == userModel.Email)
                .FirstOrDefaultAsync();

                if (user is null)
                {
                    return ResponseModel<string>.Failure("An Error occured");
                }

                string emailVerificationUri = await GetEmailVerificationUriAsync(user, _mailSettings.VerificationBaseUrl);

                string buildContent = $"Hi {user.FirstName}, " +
               $"<p>We're excited to have you get started!</p>" +
               $"<br>" +
               $"<p>First you need to confirm your account. Just click the button below.</p>" +
               $"<div class = 'row'>" +
               $"<div class = 'col-md-12 col-sm-12'>" +
               $"<a href = '{emailVerificationUri}'>" +
               $"<button class = 'btn btn-primary'>Activate account</button>" +
               $"</a>" +
               $"</div>" +
               $"</div>";
                var mailRequest = new MailRequest(
                    new List<string> { user.Email },
                    "Activate Account", MailTemplateHelper.GenerateMailContent(buildContent, "base-layout.html", companyName));
                //send mail



                await _mailService.SendAsync(mailRequest);

                return ResponseModel<string>.Success("Mail sent to email address");
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception occured while getting company plan: {ex.Message}", nameof(SendActivationMail));
                return ResponseModel<string>.Failure("An Error occured");

            }
        }

        public async Task<ResponseModel<string>> SendAccountCreationMail(UserResponseModel userModel, string companyName, string defaultPassword)
        {
            try
            {
                var user = await _userManager.Users
                    .Where(u => u.Email == userModel.Email)
                    .FirstOrDefaultAsync();

                if (user is null)
                {
                    return ResponseModel<string>.Failure("An Error occured");
                }

                string emailVerificationUri = await GetEmailVerificationUriAsync(user, _mailSettings.VerificationBaseUrl);

                string loginlink = _mailSettings.VerificationBaseUrl ?? "";
                string buildContent = $"Hi {user.FirstName}, " +
                                      $"<p>Welcome to {companyName}! We are excited to have you started</p>" +
                                      $"<br>" +
                                      $"<p>you can login to your account here. Just click the button below.</p>" +
                                      $"<p>Your default password is {defaultPassword} </p>" +
                                      $"<div class = 'row'>" +
                                      $"<div class = 'col-md-12 col-sm-12'>" +
                                      $"<a href = '{emailVerificationUri}'>" +
                                      $"<button class = 'btn btn-primary'>Login account</button>" +
                                      $"</a>" +
                                      $"</div>" +
                                      $"</div>";
                var mailRequest = new MailRequest(
                    new List<string> { user.Email },
                    "Activate Account", buildContent);
                //send mail

                mailRequest.Bcc.Add("ajepetobi@gmail.com");

                await _mailService.SendAsync(mailRequest);

                return ResponseModel<string>.Success("Mail sent to email address");
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception occured while getting company plan: {ex.Message}", nameof(SendActivationMail));
                return ResponseModel<string>.Failure("An Error occured");

            }
        }

        public async Task<ResponseModel<string>> SendApplicantAccountCreationMail(UserResponseModel userModel, string companyName, string defaultPassword)
        {
            try
            {
                var user = await _userManager.Users
                    .Where(u => u.Email == userModel.Email)
                    .FirstOrDefaultAsync();

                if (user is null)
                {
                    return ResponseModel<string>.Failure("An Error occured");
                }

                string emailVerificationUri = await GetEmailVerificationUriAsync(user, _mailSettings.VerificationBaseUrl);

                string loginlink = _mailSettings.VerificationBaseUrl ?? "";
                string buildContent = $"Hi {user.FirstName}, " +
                                      $"<p>Welcome to {companyName} recruitment portal! We are excited to have you started</p>" +
                                      $"<br>" +
                                      $"<p>you can login to your account here. Just click the button below.</p>" +
                                      $"<p>Your default password is {defaultPassword} </p>" +
                                      $"<div class = 'row'>" +
                                      $"<div class = 'col-md-12 col-sm-12'>" +
                                      $"<a href = '{emailVerificationUri}'>" +
                                      $"<button class = 'btn btn-primary'>Login account</button>" +
                                      $"</a>" +
                                      $"</div>" +
                                      $"</div>";
                var mailRequest = new MailRequest(
                    new List<string> { user.Email },
                    "Activate Account", buildContent);
                //send mail
                mailRequest.Bcc.Add("ajepetobi@gmail.com");

                await _mailService.SendAsync(mailRequest);

                return ResponseModel<string>.Success("Mail sent to email address");
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception occured while getting company plan: {ex.Message}", nameof(SendActivationMail));
                return ResponseModel<string>.Failure("An Error occured");

            }
        }
        //public async Task<ResponseModel<string>> SendApplicantCreationOTPMail(UserResponseModel userModel, string companyName, string defaultPassword)
        //{
        //    try
        //    {
        //        var user = await _userManager.Users
        //            .Where(u => u.Email == userModel.Email)
        //            .FirstOrDefaultAsync();

        //        if (user is null)
        //        {
        //            return ResponseModel<string>.Failure("An Error occured");
        //        }
        //        //GET ONE TIME CODE HERE

        //        var oneTimeCode = Generate(user.Id, expire: out DateTime expierExpireDate);


        //        string buildContent = $"Hi {user.FirstName}, " +
        //                              $"<p>Welcome to {companyName} recruitment portal! We are excited to have you started</p>" +
        //                              $"<br>" +
        //                              $"<p>Here is your one time code for your account verification</p>" +
        //                              $"<p>Your OTP {oneTimeCode} </p>" +
        //                              $"<div class = 'row'>" +
        //                              $"<div class = 'col-md-12 col-sm-12'>" +
        //                              $"<p> Your OTP expires in {expierExpireDate} </p>" +
        //                              $"</a>" +
        //                              $"</div>" +
        //                              $"</div>";
        //        var mailRequest = new MailRequest(
        //            new List<string> { user.Email },
        //            "Account Verification", buildContent);
        //        //send mail
        //        mailRequest.Bcc.Add("ajepetobi@gmail.com");

        //        await _mailService.SendAsync(mailRequest);

        //        return ResponseModel<string>.Success("OTP sent to email address");
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogCritical($"Exception occured while getting company plan: {ex.Message}", nameof(SendActivationMail));
        //        return ResponseModel<string>.Failure("An Error occured");

        //    }
        //}


        private async Task<LoginResponse> GenerateTokensAndUpdateUser(ApplicationUser user, List<string?> roleClaims, string role)
        {
            string token = GenerateJwt(user, roleClaims, role);

            user.RefreshToken = GenerateRefreshToken();
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationInDays);

            await _userManager.UpdateAsync(user);

            return new LoginResponse(token, user.RefreshToken, user.RefreshTokenExpiryTime, user?.CompanyId.ToString(), user.Id);
        }

        private string GenerateJwt(ApplicationUser user, List<string?> roleClaims, string role)
        {
            return GenerateEncryptedToken(GetSigningCredentials(), GetClaims(user, roleClaims, role));

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
            new(ClaimTypes.Name, user.FirstName ?? string.Empty),
            new(ClaimTypes.Role, role ?? string.Empty),
            new(ClaimTypes.Surname, user.LastName ?? string.Empty),
            new(TenantClaimConstants.Company, user.CompanyId.ToString()),
           // new(TenantClaimConstants.Tenant, _currentTenant!.Id),
            new(TenantClaimConstants.ImageUrl, user.ImageUrl ?? string.Empty),
            new(ClaimTypes.MobilePhone, user.PhoneNumber ?? string.Empty),
            new Claim(TenantClaimConstants.Permission, JsonConvert.SerializeObject(roleClaims))
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
                throw new InvalidOperationException("identity.invalidtoken");
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
