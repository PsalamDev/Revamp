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
    public class ScoreCardQuestionController : ControllerBase
    {
        private readonly IScoreCardQuestionService _questionService;
        public ScoreCardQuestionController(IScoreCardQuestionService questionService)
        {
            _questionService = questionService;
        }


        [HttpPut, Route("update-Score-card-question")]
        [ProducesResponseType(typeof(ResponseModel<ScoreCardQuestionModel>), 200)]
        [ProducesResponseType(typeof(ResponseModel), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateAsync([FromBody] UpdateScoreCardQuestionModel request)
        {
            var result = await _questionService.UpdateAsync(request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet, Route("load-score-card-questions")]
        [ProducesResponseType(typeof(CustomPagination<ResponseModel<List<ScoreCardQuestionModel>>>), 200)]
        [ProducesResponseType(typeof(CustomPagination<ResponseModel<List<ScoreCardQuestionModel>>>), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAll([FromQuery] PaginationRequest filter)
        {

            var result = await _questionService.GetAllAsync(filter);

            return StatusCode(result.StatusCode, result);
        }

        [HttpGet, Route("load-Score-card-question")]
        [ProducesResponseType(typeof(ResponseModel<List<ScoreCardQuestionModel>>), 200)]
        [ProducesResponseType(typeof(ResponseModel<List<ScoreCardQuestionModel>>), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> LoadQuestion([FromQuery] Guid id)
        {

            var result = await _questionService.GetSingleAsync(id);

            return StatusCode(result.StatusCode, result);
        }


        [HttpDelete, Route("delete-Score-card-question")]
        [OpenApiOperation("delete Score Crad Question")]
        [ProducesResponseType(typeof(ResponseModel<bool>), 200)]
        [ProducesResponseType(typeof(ResponseModel<bool>), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteQuestionAsync([FromQuery] Guid id)
        {
            var result = await _questionService.DeleteAsync(id);
            return StatusCode(result.StatusCode, result);
        }
    }
}