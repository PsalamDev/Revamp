using AutoMapper;
using Core.Common.Model;
using Core.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using HRShared.Common;
using Infrastructure.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Implementation
{
    public class ClaimsService : IClaimsService
    {
        private readonly IAsyncRepository<Permission, Guid> _permission;
        private readonly IMapper _mapper;
        private readonly ILogger<ClaimsService> _logger;

        public ClaimsService(IAsyncRepository<Permission, Guid> permission, IMapper mapper, ILogger<ClaimsService> logger)
        {
            _permission = permission;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<ResponseModel<ClaimsResponseModel>> CreateAsync(ClaimsRequestModel request)
        {
            try
            {

                // check if the claim name already exists

                var existClaim = await _permission.GetByAsync(x => x.ClaimName == request.ClaimName);

                if (existClaim != null)
                {
                    return ResponseModel<ClaimsResponseModel>.Failure("Claim already exists, try again");
                }
                request.Id = Guid.NewGuid();

                var permission = _mapper.Map<Permission>(request);

                await _permission.AddAsync(permission);

                await _permission.SaveChangesAsync();

                var response = _mapper.Map<ClaimsResponseModel>(permission);
                return ResponseModel<ClaimsResponseModel>.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception occured while saving claims: {ex.Message}", nameof(CreateAsync));
                return ResponseModel<ClaimsResponseModel>.Failure("Exception error");

            }
        }

        public Task<ResponseModel<bool>> DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseModel<PagedResult<ClaimsResponseModel>>> GetAllAsync(ClaimsQueryModel query, bool isAdmin = false)
        {
            try
            {
                Expression<Func<Permission, bool>> predicate = x => x.Id != null;

                if (!string.IsNullOrWhiteSpace(query.Keyword))
                {
                    predicate = predicate.And(x => x.ClaimName.ToLower() == query.Keyword.ToLower());
                }
                if (!isAdmin)
                {
                    predicate = predicate.And(x => x.IsAdmin == null || x.IsAdmin.Value == false);

                }
                var claimsPaginated = await _permission.ListWithPagingAsync(query.PageIndex, query.PageSize, predicate) ?? default!;

                return ResponseModel<PagedResult<ClaimsResponseModel>>.Success(_mapper.Map<PagedResult<ClaimsResponseModel>>(claimsPaginated));
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception occured while getting claims: {ex.Message}", nameof(GetAllAsync));
                return ResponseModel<PagedResult<ClaimsResponseModel>>.Failure("Exception error");
            }
        }

        public async Task<ResponseModel<List<ClaimsResponseModel>>> GetAllListAsync()
        {
            try
            {
                var allClaims = await _permission.ListAllAsync();
                return ResponseModel<List<ClaimsResponseModel>>.Success(_mapper.Map<List<ClaimsResponseModel>>(allClaims));
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception occured while getting claims: {ex.Message}", nameof(GetAllListAsync));
                return ResponseModel<List<ClaimsResponseModel>>.Failure("Exception error");
            }
        }

        public async Task<ResponseModel<ClaimsResponseModel>> GetSingleAsync(Guid id)
        {
            try
            {
                var claims = await _permission.GetByIdAsync(id);
                return ResponseModel<ClaimsResponseModel>.Success(_mapper.Map<ClaimsResponseModel>(claims));

            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception occured while getting claim: {ex.Message}", nameof(GetSingleAsync));
                return ResponseModel<ClaimsResponseModel>.Failure("Exception error");
            }
        }

        public async Task<ResponseModel<ClaimsResponseModel>> UpdateAsync(ClaimsRequestModel request)
        {
            try
            {
                if (request.Id == Guid.Empty)
                {
                    return ResponseModel<ClaimsResponseModel>.Failure("Invalid claim identifier");
                }

                //get the claim with that id

                var claims = await _permission.GetByIdAsync(request.Id);

                if (claims == null)
                {
                    return ResponseModel<ClaimsResponseModel>.Failure("No record of claim with Identifier found");
                }

                var checkExist = await _permission.GetFirstAsync(x => x.Id != request.Id && x.ClaimName == request.ClaimName);

                if (checkExist != null)
                {
                    return ResponseModel<ClaimsResponseModel>.Failure("Claim with same name already exist");
                }

                claims.ClaimName = request.ClaimName;
                claims.ClaimValue = request.ClaimValue;
                claims.Group = request.Group;
                claims.Description = request.Description;
                claims.IsAdmin = request.IsAdmin;

                _permission.Update(claims);
                await _permission.SaveChangesAsync();

                return ResponseModel<ClaimsResponseModel>.Success(_mapper.Map<ClaimsResponseModel>(claims));
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception occured while updating claims: {ex.Message}", nameof(UpdateAsync));
                return ResponseModel<ClaimsResponseModel>.Failure("Exception error");
            }
        }
    }
}
