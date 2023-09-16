using Core.Common.Model;
using HRShared.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IRoleClaimsService
    {
        Task<ResponseModel<RoleClaimsResponseModel>> CreateAsync(RoleClaimsRequestModel request);
        Task<ResponseModel<bool>> CreateListAsync(List<RoleClaimsRequestModel> request);
        Task<ResponseModel<RoleClaimsResponseModel>> UpdateAsync(RoleClaimsRequestModel request);
        Task<ResponseModel<RoleClaimsResponseModel>> GetSingleAsync(int id);
        Task<ResponseModel<PagedResult<RoleClaimsResponseModel>>> GetAllAsync(RoleClaimsQueryModel query);

        Task<ResponseModel<List<RoleClaimsResponseModel>>> GetRoleClaimsListAsync(Guid roleId);
        Task<ResponseModel<bool>> DeleteAsync(Guid id);
    }
}
