using AutoMapper;
using Core.Common.Model;
using Core.Extensions;
using Core.Interfaces;
using Domain.Entities.Identity;
using Domain.Interfaces;
using HRShared.Common;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Implementation
{
    public class RoleClaimsService : IRoleClaimsService
    {
        private readonly IAsyncRepository<ApplicationRoleClaim, int> _roleClaims;
        private readonly IClaimsService _claimsService;
        private readonly IRoleService _roleService;
        private readonly IMapper _mapper;
        private readonly ILogger<RoleClaimsService> _logger;

        public RoleClaimsService(IAsyncRepository<ApplicationRoleClaim, int> roleClaim, IMapper mapper, ILogger<RoleClaimsService> logger, IClaimsService claimsService, IRoleService roleService)
        {
            _roleClaims = roleClaim;
            _mapper = mapper;
            _logger = logger;
            _claimsService = claimsService;
            _roleService = roleService;
        }

        public async Task<ResponseModel<RoleClaimsResponseModel>> CreateAsync(RoleClaimsRequestModel request)
        {
            try
            {
                //check if role exist
                var roleExist = await _roleService.GetSingleAsync(request.RoleId.ToString());

                if (!roleExist.IsSuccessful || roleExist.Data == null)
                {
                    return ResponseModel<RoleClaimsResponseModel>.Failure("Specified role identifier was not found");
                }
                //check if claim exist
                var role = roleExist.Data;
                var claimExist = await _claimsService.GetSingleAsync(request.ClaimId);

                if (!claimExist.IsSuccessful || claimExist.Data == null)
                {
                    return ResponseModel<RoleClaimsResponseModel>.Failure("Specified claims identifier was not found");
                }

                var claim = claimExist.Data;
                var applicationRoleClaim = new ApplicationRoleClaim
                {
                    ClaimType = ClaimsTypeConstant.Permission,
                    ClaimValue = claim.ClaimValue,
                    CompanyId = request.CompanyId,
                    CreatedBy = request.CreatedBy.ToString(),
                    RoleId = request.RoleId.ToString(),
                    CreatedOn = DateTime.Now
                };

                await _roleClaims.AddAsync(applicationRoleClaim);

                await _roleClaims.SaveChangesAsync();

                return ResponseModel<RoleClaimsResponseModel>.Success(new RoleClaimsResponseModel
                {
                    ClaimId = request.ClaimId,
                    Claims = claim,
                    CompanyId = request.CompanyId,
                    CreatedBy = request.CreatedBy,
                    Role = role,
                    RoleId = request.RoleId
                });
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception occured while saving role claims: {ex.Message}", nameof(CreateAsync));
                return ResponseModel<RoleClaimsResponseModel>.Failure("Exception error");
            }
        }

        public async Task<ResponseModel<bool>> CreateListAsync(List<RoleClaimsRequestModel> requests)
        {
            try
            {
                var applicationRoleClaimList = new List<ApplicationRoleClaim>();

                foreach (var request in requests)
                {
                    var roleExist = await _roleService.GetSingleAsync(request.RoleId.ToString());

                    if (!roleExist.IsSuccessful || roleExist.Data == null)
                    {
                        _logger.LogError($"Role with Identifier: {request.RoleId} does not exist, looping continued to other roles");
                        continue;
                    }
                    //check if claim exist
                    var role = roleExist.Data;
                    var claimExist = await _claimsService.GetSingleAsync(request.ClaimId);

                    if (!claimExist.IsSuccessful || claimExist.Data == null)
                    {
                        _logger.LogError($"Claim with identifier: {request.ClaimId} does not exist, looping continued to other claims");
                        continue;
                    }

                    var claim = claimExist.Data;
                    var applicationRoleClaim = new ApplicationRoleClaim
                    {
                        ClaimType = ClaimsTypeConstant.Permission,
                        ClaimValue = claim.ClaimValue,
                        CompanyId = request.CompanyId,
                        CreatedBy = request.CreatedBy.ToString(),
                        RoleId = request.RoleId.ToString(),
                        CreatedOn = DateTime.Now
                    };

                    applicationRoleClaimList.Add(applicationRoleClaim);
                }

                if (applicationRoleClaimList.Count > 0)
                {
                    await _roleClaims.AddListAsync(applicationRoleClaimList);

                    await _roleClaims.SaveChangesAsync();
                    return ResponseModel<bool>.Success(true, "Operation successful");
                }

                return ResponseModel<bool>.Success(false, "Operation successful");

            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception occured while saving role claims: {ex.Message}", nameof(CreateListAsync));
                return ResponseModel<bool>.Failure("Exception error");
            }
        }

        public Task<ResponseModel<bool>> DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }


        public async Task<ResponseModel<List<RoleClaimsResponseModel>>> GetRoleClaimsListAsync(Guid roleId)
        {
            try
            {
                var roleClaims = await _roleClaims.ListAsync(predicate: x => x.RoleId == roleId.ToString());
                return ResponseModel<List<RoleClaimsResponseModel>>.Success(_mapper.Map<List<RoleClaimsResponseModel>>(roleClaims));

            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception occured while saving claims: {ex.Message}", nameof(GetRoleClaimsListAsync));
                return ResponseModel<List<RoleClaimsResponseModel>>.Failure("Exception error");
            }
        }
        public async Task<ResponseModel<PagedResult<RoleClaimsResponseModel>>> GetAllAsync(RoleClaimsQueryModel query)
        {
            try
            {
                Expression<Func<ApplicationRoleClaim, bool>> predicate = x => x.Id > 0;

                if (!string.IsNullOrWhiteSpace(query.Keyword))
                {
                    predicate = x => x.ClaimType.ToLower() == query.Keyword.ToLower();
                }
                var roleClaimsPaginated = await _roleClaims.ListWithPagingAsync(query.PageIndex, query.PageSize, predicate) ?? default!;

                return ResponseModel<PagedResult<RoleClaimsResponseModel>>.Success(_mapper.Map<PagedResult<RoleClaimsResponseModel>>(roleClaimsPaginated));
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception occured while saving claims: {ex.Message}", nameof(GetAllAsync));
                return ResponseModel<PagedResult<RoleClaimsResponseModel>>.Failure("Exception error");
            }
        }

        public async Task<ResponseModel<RoleClaimsResponseModel>> GetSingleAsync(int id)
        {
            try
            {
                var roleClaims = await _roleClaims.GetByIdAsync(id);

                var semiResponse = _mapper.Map<RoleClaimsResponseModel>(roleClaims);

                if (roleClaims == null)
                {
                    return ResponseModel<RoleClaimsResponseModel>.Success(semiResponse);
                }

                var claims = await _claimsService.GetSingleAsync(roleClaims.ClaimId);

                var role = await _roleService.GetSingleAsync(roleClaims.RoleId);

                semiResponse.Role = role.Data ?? null;
                semiResponse.Claims = claims.Data ?? null;

                return ResponseModel<RoleClaimsResponseModel>.Success(semiResponse);

            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception occured while saving claims: {ex.Message}", nameof(GetSingleAsync));
                return ResponseModel<RoleClaimsResponseModel>.Failure("Exception error");
            }
        }

        public Task<ResponseModel<RoleClaimsResponseModel>> UpdateAsync(RoleClaimsRequestModel request)
        {
            throw new NotImplementedException();
        }
    }
}
