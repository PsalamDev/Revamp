using Core.Common.Model;
using HRShared.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IRecruitmentFocusAreaService
    {
        Task<ResponseModel<RecruitmentFocusAreaModel>> CreateAsync(CreateRecruitmentFocusAreaRequestModel request);
        Task<ResponseModel<RecruitmentFocusAreaModel>> UpdateAsync(UpdateRecruitmentFocusAreaRequestModel request);
        Task<ResponseModel<RecruitmentFocusAreaModel>> GetSingleAsync(Guid id);
        Task<ResponseModel<bool>> DeleteAsync(Guid id);
        Task<ResponseModel<CustomPagination<List<RecruitmentFocusAreaDto>>>> GetAllAsync(PaginationRequest filter);
        Task<ResponseModel<List<SelectListItemDataModel>>> LoadRecruitmentFocusAreaSelectListItem(string? filter);
    }
}
