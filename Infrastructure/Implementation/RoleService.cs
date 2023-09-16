using AutoMapper;
using Core.Common.Model;
using Core.Constants;
using Core.Interfaces;
using Domain.Entities.Identity;
using HRShared.Common;
using Infrastructure.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Implementation
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ICurrentUser _currentUser;
        private readonly IMapper _mapper;
        private readonly ILogger<RoleService> _logger;

        public RoleService(RoleManager<ApplicationRole> roleManager, ICurrentUser currentUser, IMapper mapper, ILogger<RoleService> logger)
        {
            _roleManager = roleManager;
            _mapper = mapper;
            _logger = logger;
            _currentUser = currentUser;
        }

        public async Task<ResponseModel<RoleResponseModel>> CreateAsync(RoleRequestModel request)
        {

            try
            {
                var roleNameExist = await _roleManager.Roles.FirstOrDefaultAsync(x => x.Name == request.Name && x.CompanyId == request.CompanyId);
                if (roleNameExist != null)
                {
                    return ResponseModel<RoleResponseModel>.Failure($"{request.Name} already exists");
                }

                var applicationRole = new ApplicationRole
                {
                    CompanyId = request.CompanyId,
                    Name = request.Name,
                    Description = request.Description
                };



                await _roleManager.CreateAsync(applicationRole);

                return ResponseModel<RoleResponseModel>.Success(_mapper.Map<RoleResponseModel>(applicationRole));

            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception occured while saving role: {ex.Message}", nameof(CreateAsync));
                return ResponseModel<RoleResponseModel>.Failure("Exception error");
            }
        }

        public Task<ResponseModel<bool>> DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }


        public async Task<ResponseModel<List<RoleResponseModel>>> GetAllAsync(RoleQueryModel query)
        {
            try
            {
                Expression<Func<ApplicationRole, bool>> predicate = x => x.Id != null;


                if (!string.IsNullOrWhiteSpace(query.Keyword))
                {
                    predicate = x => x.Name.ToLower().Contains(query.Keyword.ToLower());
                }

                var rolePaginated = await _roleManager.Roles.ToListAsync();

                return ResponseModel<List<RoleResponseModel>>.Success(_mapper.Map<List<RoleResponseModel>>(rolePaginated));
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception occured while getting roles: {ex.Message}", nameof(GetAllAsync));
                return ResponseModel<List<RoleResponseModel>>.Failure("Exception error");
            }
        }

        public async Task<ResponseModel<RoleResponseModel>> GetSingleAsync(string id)
        {
            try
            {
                var role = await _roleManager.Roles.FirstOrDefaultAsync(x => x.Id == id);
                return ResponseModel<RoleResponseModel>.Success(_mapper.Map<RoleResponseModel>(role));

            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception occured while getting role: {ex.Message}", nameof(GetSingleAsync));
                return ResponseModel<RoleResponseModel>.Failure("Exception error");
            }
        }

        public async Task<ResponseModel<RoleResponseModel>> GetSingleByNameAsync(string roleName)
        {
            try
            {
                var role = await _roleManager.Roles.FirstOrDefaultAsync(x => x.Name.Equals(roleName));
                return ResponseModel<RoleResponseModel>.Success(_mapper.Map<RoleResponseModel>(role));

            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception occured while getting role: {ex.Message}", nameof(GetSingleAsync));
                return ResponseModel<RoleResponseModel>.Failure("Exception error");
            }
        }

        public async Task<ResponseModel<RoleResponseModel>> UpdateAsync(RoleRequestModel request)
        {
            try
            {
                if (request.Id == Guid.Empty)
                {
                    return ResponseModel<RoleResponseModel>.Failure("Invalid claim identifier");
                }

                //get the claim with that id

                var role = _roleManager.Roles.FirstOrDefault(x => x.Id == request.Id.ToString());

                if (role == null)
                {
                    return ResponseModel<RoleResponseModel>.Failure("No record of role with Identifier found");
                }

                var checkExist = _roleManager.Roles.FirstOrDefault(x => x.Id != request.Id.ToString() && x.Name == request.Name);

                if (checkExist != null)
                {
                    return ResponseModel<RoleResponseModel>.Failure("Role with same name already exist");
                }

                role.Name = request.Name;
                role.Description = request.Description;

                await _roleManager.UpdateAsync(role);

                return ResponseModel<RoleResponseModel>.Success(_mapper.Map<RoleResponseModel>(role));
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception occured while updating roles: {ex.Message}", nameof(UpdateAsync));
                return ResponseModel<RoleResponseModel>.Failure("Exception error");
            }
        }


        internal static List<string> DefaultRoles =>
            typeof(DefaultRoleConstant).GetAllPublicConstantValues<string>();
    }

}
