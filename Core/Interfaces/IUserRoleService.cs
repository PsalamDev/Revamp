using Core.Common.Model;
using HRShared.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IUserRoleService
    {
        Task<ResponseModel<UserRoleResponseModel>> CreateAsync(UserRoleRequestModel request);
        Task<ResponseModel<UserRoleResponseModel>> UpdateAsync(UserRoleRequestModel request);
        Task<ResponseModel<UserRoleResponseModel>> GetSingleAsyncByRoleId(string roleId);
        Task<ResponseModel<UserRoleResponseModel>> GetByUserAsync(Guid userId);
        Task<ResponseModel<PagedResult<UserRoleResponseModel>>> GetAllAsync(UserRoleQueryModel query);
        Task<ResponseModel<bool>> DeleteAsync(Guid id);
    }
}
