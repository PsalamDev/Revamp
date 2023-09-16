using Azure;
using HRShared.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using Domain.Entities;
using Domain.Interfaces;
using Core.Interfaces;
using Core.Common.Model.RecruitmentDto;
using Core.Common.Model;
namespace Service.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ScoreCardController : ControllerBase
    {
        private readonly IScoreCardService _scoreCardService;

        public ScoreCardController(IScoreCardService scoreCardService)
        {
            _scoreCardService = scoreCardService;
        }

        [HttpPost, Route("add-score-card")]
        [ProducesResponseType(typeof(ResponseModel<ScoreCardModel>), 200)]
        [ProducesResponseType(typeof(ResponseModel), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateAsync([FromBody] CreateScoreCardRequestModel request)
        {

            var result = await _scoreCardService.CreateAsync(request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut, Route("update-score-card")]
        [ProducesResponseType(typeof(ResponseModel<ScoreCardModel>), 200)]
        [ProducesResponseType(typeof(ResponseModel), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateAsync([FromBody] UpdateScoreCardRequestModel request)
        {
            var result = await _scoreCardService.UpdateAsync(request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet, Route("load-score-cards")]
        [ProducesResponseType(typeof(ResponseModel<CustomPagination<List<ScoreCardModel>>>), 200)]
        [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAll([FromQuery] PaginationRequest filter)
        {

            var result = await _scoreCardService.GetAllAsync(filter);

            return StatusCode(result.StatusCode, result);
        }

        [HttpGet, Route("load-score-card")]
        [ProducesResponseType(typeof(ResponseModel<List<ScoreCardModel>>), 200)]
        [ProducesResponseType(typeof(ResponseModel<List<ScoreCardModel>>), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> LoadScoreCard([FromQuery] Guid id)
        {
            var result = await _scoreCardService.GetSingleAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete, Route("delete-score-card")]
        [ProducesResponseType(typeof(ResponseModel<bool>), 200)]
        [ProducesResponseType(typeof(ResponseModel<bool>), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteStageAsync([FromQuery] Guid id)
        {

            var result = await _scoreCardService.DeleteAsync(id);
            return StatusCode(result.StatusCode, result);
        }


        [HttpGet, Route("load-score-card-select")]
        [ProducesResponseType(typeof(ResponseModel<SelectListItemDataModel>), 200)]
        [ProducesResponseType(typeof(ResponseModel<SelectListItemDataModel>), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> LoadHiringStageForSelectAsync([FromQuery] string? q = null)
        {
            q ??= string.Empty;
            try
            {
                var stages = await _scoreCardService.LoadScoreCardSelectListItem(q);
                return Ok(stages.Data.Where(c => c.Name!.Contains(q,
                                                              StringComparison.OrdinalIgnoreCase)));
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = "error",
                    ex.Message
                });
            }
        }
    }
}