using AutoMapper;
using Core.Common.Model;
using Core.Interfaces;
using Domain.Entities.Identity;
using Domain.Interfaces;
using HRShared.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Implementation
{
    internal class UserRoleService : IUserRoleService
    {
        private readonly IAsyncRepository<IdentityUserRole<string>, string> _userRole;

        private readonly IIdentityService _userManager;
        private readonly IRoleService _roleService;
        private readonly IRoleClaimsService _roleClaimsService;
        private readonly IMapper _mapper;
        private readonly ILogger<UserRoleService> _logger;

        public UserRoleService(IAsyncRepository<IdentityUserRole<string>, string> userRole, IIdentityService userManager, IRoleService roleService, IRoleClaimsService roleClaimsService, IMapper mapper, ILogger<UserRoleService> logger)
        {
            _userRole = userRole;
            _roleService = roleService;
            _mapper = mapper;
            _logger = logger;
            _userManager = userManager;
            _roleClaimsService = roleClaimsService;
        }

        public async Task<ResponseModel<UserRoleResponseModel>> CreateAsync(UserRoleRequestModel request)
        {
            try
            {
                var user = await _userManager.GetSingle(request.UserId);
                if (!user.IsSuccessful && user.Data == null)
                {
                    return ResponseModel<UserRoleResponseModel>.Failure("User record not found");
                }

                var role = await _roleService.GetSingleAsync(request.RoleId.ToString());
                if (!role.IsSuccessful && role.Data == null)
                {
                    return ResponseModel<UserRoleResponseModel>.Failure("Role record not found");
                }

                var roleClaims = await _roleClaimsService.GetRoleClaimsListAsync(request.RoleId);
                if (roleClaims.Data != null && roleClaims.Data.Count > 0)
                {
                    var containsAdminsClaim = roleClaims.Data.Any(x => x.Claims?.IsAdmin.Value ?? false);
                    if (containsAdminsClaim)
                    {
                        return ResponseModel<UserRoleResponseModel>.Failure("Role cannot be applied to user");

                    }
                }

                var checkIfExist = await _userRole.GetByAsync(x => x.UserId == request.UserId.ToString());

                if (checkIfExist != null)
                {
                    return ResponseModel<UserRoleResponseModel>.Failure("User already has a role");
                }

                var applicationUserRole = new ApplicationUserRole
                {
                    RoleId = request.RoleId.ToString(),
                    UserId = request.UserId.ToString(),
                };

                await _userRole.AddAsync(applicationUserRole);

                await _userRole.SaveChangesAsync();

                return ResponseModel<UserRoleResponseModel>.Success(new UserRoleResponseModel
                {
                    RoleId = request.RoleId,
                    UserId = request.UserId,
                    RoleModel = role.Data,
                    UserModel = user.Data
                });

            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception occured while assigning role to user: {ex.Message}", nameof(CreateAsync));
                return ResponseModel<UserRoleResponseModel>.Failure("Exception error");
            }
        }

        public Task<ResponseModel<bool>> DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseModel<PagedResult<UserRoleResponseModel>>> GetAllAsync(UserRoleQueryModel query)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseModel<UserRoleResponseModel>> GetByUserAsync(Guid userId)
        {
            try
            {
                var obj = await _userRole.GetByAsync(x => x.UserId == userId.ToString());

                if (obj == null)
                {
                    return ResponseModel<UserRoleResponseModel>.Failure("User role not found");
                }

                var user = await _userManager.GetSingle(Guid.Parse(obj.UserId));
                if (!user.IsSuccessful && user.Data == null)
                {
                    return ResponseModel<UserRoleResponseModel>.Failure("User record not found");
                }

                var role = await _roleService.GetSingleAsync(obj.RoleId);
                if (!role.IsSuccessful && role.Data == null)
                {
                    return ResponseModel<UserRoleResponseModel>.Failure("Role record not found");
                }

                return ResponseModel<UserRoleResponseModel>.Success(new UserRoleResponseModel
                {
                    RoleId = Guid.Parse(obj.RoleId),
                    UserId = Guid.Parse(obj.UserId),
                    RoleModel = role.Data,
                    UserModel = user.Data
                });


            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception occured while getting user role: {ex.Message}", nameof(GetByUserAsync));
                return ResponseModel<UserRoleResponseModel>.Failure("Exception error");
            }
        }

        public async Task<ResponseModel<UserRoleResponseModel>> GetSingleAsyncByRoleId(string roleId)
        {
            try
            {
                var obj = await _userRole.GetByAsync(x => x.RoleId == roleId);

                if (obj == null)
                {
                    return ResponseModel<UserRoleResponseModel>.Failure("User role not found");
                }

                var user = await _userManager.GetSingle(Guid.Parse(obj.UserId));
                if (!user.IsSuccessful && user.Data == null)
                {
                    return ResponseModel<UserRoleResponseModel>.Failure("User record not found");
                }

                var role = await _roleService.GetSingleAsync(obj.RoleId);
                if (!role.IsSuccessful && role.Data == null)
                {
                    return ResponseModel<UserRoleResponseModel>.Failure("Role record not found");
                }

                return ResponseModel<UserRoleResponseModel>.Success(new UserRoleResponseModel
                {
                    RoleId = Guid.Parse(obj.RoleId),
                    UserId = Guid.Parse(obj.UserId),
                    RoleModel = role.Data,
                    UserModel = user.Data
                });


            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception occured while getting user role: {ex.Message}", nameof(GetSingleAsyncByRoleId));
                return ResponseModel<UserRoleResponseModel>.Failure("Exception error");
            }
        }

        public Task<ResponseModel<UserRoleResponseModel>> UpdateAsync(UserRoleRequestModel request)
        {
            throw new NotImplementedException();
        }
    }
}

