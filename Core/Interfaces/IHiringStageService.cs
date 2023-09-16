using Core.Common.Model;
using HRShared.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IHiringStageService
    {
        Task<ResponseModel<StageModel>> CreateAsync(CreateStageRequestModel request);
        Task<ResponseModel<StageModel>> UpdateAsync(UpdateStageRequestModel request);
        Task<ResponseModel<StageModel>> GetSingleAsync(Guid id);
        Task<ResponseModel<bool>> DeleteAsync(Guid Id);
        Task<ResponseModel<CustomPagination<List<StagesModel>>>> GetAllAsync(PaginationRequest filter);
        Task<ResponseModel<List<SelectListItemDataModel>>> LoadStageSelectListItem(string? filter);
        Task<ResponseModel<string>> CreateStagesSeederAsync(Guid cId);

    }
}
