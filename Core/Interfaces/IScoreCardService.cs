using Core.Common.Model;
using HRShared.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IScoreCardService
    {
        Task<ResponseModel<ScoreCardModel>> CreateAsync(CreateScoreCardRequestModel request);
        Task<ResponseModel<ScoreCardModel>> UpdateAsync(UpdateScoreCardRequestModel request);
        Task<ResponseModel<ScoreCardDto>> GetSingleAsync(Guid id);
        Task<ResponseModel<bool>> DeleteAsync(Guid Id);
        Task<ResponseModel<CustomPagination<List<ScoreCardDto>>>> GetAllAsync(PaginationRequest filter);
        Task<ResponseModel<List<SelectListItemDataModel>>> LoadScoreCardSelectListItem(string? filter);
    }
}
