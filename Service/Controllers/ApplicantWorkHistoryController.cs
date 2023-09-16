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
    public class ApplicantWorkHistoryController : BaseController
    {
        private readonly IApplicantWorkHistory _applicantWorkService;
        public ApplicantWorkHistoryController(IApplicantWorkHistory applicantWorkService)
        {
            _applicantWorkService = applicantWorkService;
        }

        [HttpPost]
        [OpenApiOperation("create applicant history", "An endpoint for creating new applicant history")]
        [ProducesResponseType(typeof(ResponseModel<ApplicantProfileResponse>), 200)]
        [ProducesResponseType(typeof(ResponseModel), 400)]
        public async Task<IActionResult> CreateAsync([FromBody] ApplicantHistoryRequest request)
        {
            var result = await _applicantWorkService.CreateAsync(request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut]
        [OpenApiOperation("Update applicant history", "An endpoint for updating  applicant history")]
        [ProducesResponseType(typeof(ResponseModel<ApplicantProfileResponse>), 200)]
        [ProducesResponseType(typeof(ResponseModel), 400)]
        public async Task<IActionResult> CreateAsync([FromBody] UpdateApplicantHistoryRequest request)
        {
            var result = await _applicantWorkService.UpdateAsync(request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete]
        [OpenApiOperation("delete applicant history", "An endpoint for delete applicant")]
        [ProducesResponseType(typeof(ResponseModel<ApplicantProfileResponse>), 200)]
        [ProducesResponseType(typeof(ResponseModel), 400)]
        public async Task<IActionResult> DeleteHistory([FromBody] Guid Id)
        {
            var result = await _applicantWorkService.DeleteAsync(Id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet, Route("GetSingleHistory")]
        [OpenApiOperation("Get single history", "Single Applicant history")]
        [ProducesResponseType(typeof(ResponseModel<ApplicantProfileResponse>), 200)]
        [ProducesResponseType(typeof(ResponseModel<ApplicantProfileResponse>), 400)]
        public async Task<IActionResult> GetSingle([FromQuery] Guid HistoryId)
        {
            var result = await _applicantWorkService.GetSingleAsync(HistoryId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet, Route("GetApplicantHistories")]
        [OpenApiOperation("Get Applicant History List", "History List")]
        [ProducesResponseType(typeof(ResponseModel<CustomPagination<List<ApplicantProfileListResponse>>>), 200)]
        [ProducesResponseType(typeof(ResponseModel<CustomPagination<List<ApplicantProfileListResponse>>>), 400)]
        public async Task<IActionResult> GetListById([FromQuery] GetHistoryRequestList req)
        {
            var result = await _applicantWorkService.GetAllListAsync(req);
            return StatusCode(result.StatusCode, result);
        }


    }
}