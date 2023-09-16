using Core.Common.Model;
using Core.Interfaces;
using HRShared.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace Service.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class QuizController : ControllerBase
    {
        private readonly IQuizService _quizService;
        public QuizController(IQuizService quizService)
        {
            _quizService = quizService;
        }

        #region Type: PsychometricTypes:

        /// <summary>
        /// API for getting Quiz Types i.e.
        /// {Interest, Personality, and  Aptitude Tests}
        /// </summary>
        [HttpGet]
        [Route("GetQuizTypes")]
        [ProducesResponseType(typeof(IList<IDTextViewModel>), 200)]
        [ProducesResponseType(typeof(IList<IDTextViewModel>), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> FetchQuizTypes()
        {
            var result = await _quizService.GetQuizTypes();

            return Ok(result);
        }


        /// <summary>
        /// API for getting Question Types i.e.
        /// {'Multiple Choice, True/False, Multiple Answers, Fill-in-the-Blank'} 
        /// </summary>
        [HttpGet]
        [OpenApiOperation("Get Question Type", "Get Question Type")]
        [Route("GetQuestionTypes")]
        [ProducesResponseType(typeof(IList<IDTextViewModel>), 200)]
        [ProducesResponseType(typeof(IList<IDTextViewModel>), 400)]
        [ProducesResponseType(500)]
        public IActionResult FetchQuestionTypes()
        {

            var result = _quizService.GetQuestionTypes();

            return Ok(result);
        }

        #endregion


        #region Quiz & Question-Options:

        /// <summary>
        /// API for adding/updating Quiz
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("Add-Update-Quiz")]
        [ProducesResponseType(typeof(ResponseModel<bool>), 200)]
        [ProducesResponseType(typeof(ResponseModel<bool>), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> AddUpdateQuiz([FromBody] ManageQuizDTO payload)
        {

            var result = await _quizService.AddUpdateQuiz(payload);

            return StatusCode(result.StatusCode, result);
        }



        /// <summary>
        /// API for submitting Quiz
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("SubmitQuizTest")]
        [ProducesResponseType(typeof(ResponseModel<bool>), 200)]
        [ProducesResponseType(typeof(ResponseModel<bool>), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> SubmitQuizTest([FromBody] QuizAnswerDTO payload)
        {

            var result = await _quizService.SubmitQuiz(payload);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("StartQuiz")]
        [ProducesResponseType(typeof(ResponseModel<QuizDTO>), 200)]
        [ProducesResponseType(typeof(ResponseModel<QuizDTO>), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> StartQuiz([FromQuery] Guid quizId)
        {
            var result = await _quizService.StartQuiz(quizId);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// API to Fetch Quizzes.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [OpenApiOperation("Get GetAllQuizzes", "Get All Quizzes List")]
        [Route("GetAllQuizzes")]
        [ProducesResponseType(typeof(ResponseModel<CustomPagination<List<QuizDTO>>>), 200)]
        [ProducesResponseType(typeof(ResponseModel<CustomPagination<List<QuizDTO>>>), 400)]

        public async Task<IActionResult> GetAllQuizzes([FromQuery] Quizfilter filter)
        {

            var result = await _quizService.GetAllQuizzes(filter);

            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// API to get Quiz by id and can be used for update, details etc
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("GetQuiz")]
        [ProducesResponseType(typeof(ResponseModel<QuizDTO>), 200)]
        [ProducesResponseType(typeof(ResponseModel<QuizDTO>), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetQuizById([FromQuery] Guid id)
        {
            var result = await _quizService.GetQuizById(id);

            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// API to delete all quiz by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        [HttpDelete("DeleteQuiz")]
        [ProducesResponseType(typeof(ResponseModel<bool>), 200)]
        [ProducesResponseType(typeof(ResponseModel<bool>), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteQuiz([FromQuery] Guid id)
        {

            var result = await _quizService.DeleteQuiz(id);
            return StatusCode(result.StatusCode, result);
        }


        /// <summary>
        /// API to Fetch Applicant Quizzes.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [OpenApiOperation("Get GetAllApplicantQuizzes", "Get All Applicant Quizzes List")]
        [Route("GetAllApplicantQuizzes")]
        [ProducesResponseType(typeof(ResponseModel<CustomPagination<List<QuizDTO>>>), 200)]
        [ProducesResponseType(typeof(ResponseModel<CustomPagination<List<QuizDTO>>>), 400)]

        public async Task<IActionResult> GetAllApplicantQuizzes([FromQuery] ApplicantQuizfilter filter)
        {

            var result = await _quizService.GetApplicantAllQuizzes(filter);

            return StatusCode(result.StatusCode, result);
        }

        #endregion

    }
}