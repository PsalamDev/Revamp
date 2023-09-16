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
    public class RecruitmentFocusAreaController : ControllerBase
    {
        private readonly IRecruitmentFocusAreaService _recruitmentFocusAreaService;
        public RecruitmentFocusAreaController(IRecruitmentFocusAreaService recruitmentFocusAreaService)
        {
            _recruitmentFocusAreaService = recruitmentFocusAreaService;
        }

        [HttpPost, Route("add-recruitment-focus-area")]
        [ProducesResponseType(typeof(ResponseModel<RecruitmentFocusAreaModel>), 200)]
        [ProducesResponseType(typeof(ResponseModel<RecruitmentFocusAreaModel>), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateAsync([FromBody] CreateRecruitmentFocusAreaRequestModel request)
        {

            var result = await _recruitmentFocusAreaService.CreateAsync(request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut, Route("update-recruitment-focus-area")]
        [ProducesResponseType(typeof(ResponseModel<RecruitmentFocusAreaModel>), 200)]
        [ProducesResponseType(typeof(ResponseModel<RecruitmentFocusAreaModel>), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateAsync([FromBody] UpdateRecruitmentFocusAreaRequestModel request)
        {
            var result = await _recruitmentFocusAreaService.UpdateAsync(request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet, Route("load-recruitment-focus-areas")]
        [ProducesResponseType(typeof(ResponseModel<CustomPagination<List<RecruitmentFocusAreaDto>>>), 200)]
        [ProducesResponseType(typeof(ResponseModel<CustomPagination<List<RecruitmentFocusAreaDto>>>), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAll([FromQuery] PaginationRequest filter)
        {

            var result = await _recruitmentFocusAreaService.GetAllAsync(filter);

            return StatusCode(result.StatusCode, result);
        }

        [HttpGet, Route("load-recruitment-focus-area")]
        [ProducesResponseType(typeof(ResponseModel<List<RecruitmentFocusAreaModel>>), 200)]
        [ProducesResponseType(typeof(ResponseModel<List<RecruitmentFocusAreaModel>>), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> LoadRecruitmentFocusArea([FromQuery] Guid id)
        {

            var result = await _recruitmentFocusAreaService.GetSingleAsync(id);

            return StatusCode(result.StatusCode, result);
        }


        [HttpDelete, Route("delete-recruitment-focus-area")]
        [ProducesResponseType(typeof(ResponseModel<bool>), 200)]
        [ProducesResponseType(typeof(ResponseModel<bool>), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteRecruitmentFocusAreaAsync([FromQuery] Guid id)
        {

            var result = await _recruitmentFocusAreaService.DeleteAsync(id);
            return StatusCode(result.StatusCode, result);
        }




        [HttpGet, Route("load-recruitment-focus-area-select")]
        [ProducesResponseType(typeof(ResponseModel<SelectListItemDataModel>), 200)]
        [ProducesResponseType(typeof(ResponseModel<SelectListItemDataModel>), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> LoadFocusAreaTypeForSelectAsync([FromQuery] string? q = null)
        {
            q ??= string.Empty;
            try
            {
                var focusArea = await _recruitmentFocusAreaService.LoadRecruitmentFocusAreaSelectListItem(q);
                return Ok(focusArea.Data.Where(c => c.Name!.Contains(q,
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