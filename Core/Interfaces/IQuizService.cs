using Core.Common.Model;
using HRShared.Common;

namespace Core.Interfaces
{
    public interface IQuizService
    {
        Task<ResponseModel<bool>> AddUpdateQuiz(ManageQuizDTO payload);
        Task<ResponseModel<bool>> SubmitQuiz(QuizAnswerDTO payload);
        Task<ResponseModel<QuizDTO>> StartQuiz(Guid quizId);
        Task<ResponseModel<CustomPagination<List<QuizDTO>>>> GetAllQuizzes(Quizfilter filter);
        //Task<ResponseModel<CustomPagination<List<Quiz>>>> GetQuizzes(int companyId);
        Task<ResponseModel<QuizDTO>> GetQuizById(Guid id);
        Task<ResponseModel<bool>> DeleteQuiz(Guid id);
        Task<ResponseModel<CustomPagination<List<QuizDTO>>>> GetApplicantAllQuizzes(ApplicantQuizfilter filter);


        #region Type: PsychometricTypes:

        Task<List<IDTextViewModel>> GetQuizTypes();
        List<IDTextViewModel> GetQuestionTypes();

        #endregion
    }
}