using Core.Common.Model;
using HRShared.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IHiringSubStageService
    {
        Task<ResponseModel<SubStageModel>> CreateAsync(CreateSubStageRequestModel request);
        Task<ResponseModel<SubStageModel>> UpdateAsync(UpdateSubStageRequestModel request);
        Task<ResponseModel<SubStageModel>> GetSingleAsync(Guid id);
        Task<ResponseModel<bool>> DeleteAsync(Guid Id);
        Task<ResponseModel<CustomPagination<List<SubStagesModel>>>> GetAllAsync(PaginationRequest filter);
    }
}
