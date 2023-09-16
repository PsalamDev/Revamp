using Core.Common.Model;
using HRShared.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Core.Interfaces
{
    public interface IScoreCardQuestionService
    {

        Task<ResponseModel<bool>> DeleteAsync(Guid id);
        Task<ResponseModel<ScoreCardQuestionModel>> GetSingleAsync(Guid id);
        Task<ResponseModel<CustomPagination<List<ScoreCardQuestionModel>>>> GetAllAsync(PaginationRequest filter);
        Task<ResponseModel<ScoreCardQuestionModel>> UpdateAsync(UpdateScoreCardQuestionModel request);
    }
}
