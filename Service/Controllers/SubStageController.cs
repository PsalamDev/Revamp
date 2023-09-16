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
    public class SubStageController : ControllerBase
    {
        private readonly IHiringSubStageService _hiringSubStageService;
        public SubStageController(IHiringSubStageService hiringSubStageService)
        {
            _hiringSubStageService = hiringSubStageService;
        }

        [HttpPost, Route("load-sub-stage")]
        [ProducesResponseType(typeof(ResponseModel<SubStageModel>), 200)]
        [ProducesResponseType(typeof(ResponseModel), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateAsync([FromBody] CreateSubStageRequestModel request)
        {

            var result = await _hiringSubStageService.CreateAsync(request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut, Route("update-sub-stage")]
        [ProducesResponseType(typeof(ResponseModel<SubStageModel>), 200)]
        [ProducesResponseType(typeof(ResponseModel), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateAsync([FromBody] UpdateSubStageRequestModel request)
        {
            var result = await _hiringSubStageService.UpdateAsync(request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet, Route("load-sub-stages")]
        [ProducesResponseType(typeof(ResponseModel<CustomPagination<List<SubStagesModel>>>), 200)]
        [ProducesResponseType(typeof(ResponseModel<CustomPagination<List<SubStagesModel>>>), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAll([FromQuery] PaginationRequest filter)
        {

            var result = await _hiringSubStageService.GetAllAsync(filter);

            return StatusCode(result.StatusCode, result);
        }

        [HttpGet, Route("load-sub-stage")]
        [OpenApiOperation("Get Sub Stage")]
        [ProducesResponseType(typeof(ResponseModel<List<SubStageModel>>), 200)]
        [ProducesResponseType(typeof(ResponseModel<List<SubStageModel>>), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> RecruitmentSubStage([FromQuery] Guid id)
        {

            var result = await _hiringSubStageService.GetSingleAsync(id);

            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete, Route("delete-sub-stage")]
        [OpenApiOperation("delete Sub Stage")]
        [ProducesResponseType(typeof(ResponseModel<bool>), 200)]
        [ProducesResponseType(typeof(ResponseModel<bool>), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteSubStageAsync([FromQuery] Guid id)
        {

            var result = await _hiringSubStageService.DeleteAsync(id);
            return StatusCode(result.StatusCode, result);
        }
    }
}
