using Core.Common.Model;
using HRShared.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IRoleService
    {
        Task<ResponseModel<RoleResponseModel>> CreateAsync(RoleRequestModel request);
        Task<ResponseModel<RoleResponseModel>> UpdateAsync(RoleRequestModel request);
        Task<ResponseModel<RoleResponseModel>> GetSingleAsync(string id);
        Task<ResponseModel<RoleResponseModel>> GetSingleByNameAsync(string roleName);
        Task<ResponseModel<List<RoleResponseModel>>> GetAllAsync(RoleQueryModel query);
        Task<ResponseModel<bool>> DeleteAsync(Guid id);
    }
}
