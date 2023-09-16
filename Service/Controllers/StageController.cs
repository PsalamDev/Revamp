using Core.Common.Model;
using Core.Interfaces;
using HRShared.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Service.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class StageController : ControllerBase
    {
        private readonly IHiringStageService _hiringStageService;
        public StageController(IHiringStageService hiringStageService)
        {
            _hiringStageService = hiringStageService;
        }

        [HttpPost, Route("add-stage")]
        [ProducesResponseType(typeof(ResponseModel<StageModel>), 200)]
        [ProducesResponseType(typeof(ResponseModel<StageModel>), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateAsync([FromBody] CreateStageRequestModel request)
        {

            var result = await _hiringStageService.CreateAsync(request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut, Route("update-stage")]
        [ProducesResponseType(typeof(ResponseModel<StageModel>), 200)]
        [ProducesResponseType(typeof(ResponseModel<StageModel>), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateAsync([FromBody] UpdateStageRequestModel request)
        {
            var result = await _hiringStageService.UpdateAsync(request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet, Route("load-stages")]
        [ProducesResponseType(typeof(ResponseModel<CustomPagination<List<StagesModel>>>), 200)]
        [ProducesResponseType(typeof(ResponseModel<CustomPagination<List<StagesModel>>>), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAll([FromQuery] PaginationRequest filter)
        {

            var result = await _hiringStageService.GetAllAsync(filter);

            return StatusCode(result.StatusCode, result);
        }

        [HttpGet, Route("load-stage")]
        [ProducesResponseType(typeof(ResponseModel<List<StageModel>>), 200)]
        [ProducesResponseType(typeof(ResponseModel<List<StageModel>>), 400)]
        public async Task<IActionResult> RecruitmentStage([FromQuery] Guid id)
        {

            var result = await _hiringStageService.GetSingleAsync(id);

            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete, Route("delete-stage")]
        [ProducesResponseType(typeof(ResponseModel<bool>), 200)]
        [ProducesResponseType(typeof(ResponseModel<bool>), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteStageAsync([FromQuery] Guid id)
        {

            var result = await _hiringStageService.DeleteAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet, Route("load-stage-select")]
        [ProducesResponseType(typeof(ResponseModel<SelectListItemDataModel>), 200)]
        [ProducesResponseType(typeof(ResponseModel<SelectListItemDataModel>), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> LoadHiringStageForSelectAsync([FromQuery] string? q = null)
        {
            q ??= string.Empty;
            try
            {
                var stages = await _hiringStageService.LoadStageSelectListItem(q);
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

        [HttpPost, Route("HiringStagesSeeder")]
        [ProducesResponseType(typeof(ResponseModel<string>), 200)]
        [ProducesResponseType(typeof(ResponseModel<string>), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> SeedHiringStagesAsync([FromQuery] Guid companyId)
        {
            var result = await _hiringStageService.CreateStagesSeederAsync(companyId);
            return StatusCode(result.StatusCode, result);
        }
    }
}
