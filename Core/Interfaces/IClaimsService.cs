using Core.Common.Model;
using HRShared.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IClaimsService
    {
        Task<ResponseModel<ClaimsResponseModel>> CreateAsync(ClaimsRequestModel request);
        Task<ResponseModel<ClaimsResponseModel>> UpdateAsync(ClaimsRequestModel request);
        Task<ResponseModel<ClaimsResponseModel>> GetSingleAsync(Guid id);
        Task<ResponseModel<PagedResult<ClaimsResponseModel>>> GetAllAsync(ClaimsQueryModel query, bool isAdmin = false);
        Task<ResponseModel<List<ClaimsResponseModel>>> GetAllListAsync();
        Task<ResponseModel<bool>> DeleteAsync(Guid id);
    }
}
