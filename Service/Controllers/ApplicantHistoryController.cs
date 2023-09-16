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
    public class ApplicantHistoryController : ControllerBase
    {
        private readonly IApplicantWorkHistory _applicantWorkService;

        public ApplicantHistoryController(IApplicantWorkHistory applicantWorkService)
        {
            _applicantWorkService = applicantWorkService;
        }


        [HttpPost]
        [OpenApiOperation("add applicants work history", "An endpoint for adding new applicant history")]
        [ProducesResponseType(typeof(ResponseModel<ApplicantHistoryResponse>), 200)]
        [ProducesResponseType(typeof(ResponseModel), 400)]
        public async Task<IActionResult> CreateAsync([FromBody] ApplicantHistoryRequest request)
        {
            var result = await _applicantWorkService.CreateAsync(request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete]
        [OpenApiOperation("delete applicant history", "An endpoint for deleting  applicant history")]
        [ProducesResponseType(typeof(ResponseModel<bool>), 200)]
        [ProducesResponseType(typeof(ResponseModel), 400)]
        public async Task<IActionResult> DeleteAsync([FromQuery] Guid Id)
        {
            var result = await _applicantWorkService.DeleteAsync(Id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut]
        [OpenApiOperation("Update applicant history", "An endpoint for updating  applicants work history")]
        [ProducesResponseType(typeof(ResponseModel<ApplicantHistoryResponse>), 200)]
        [ProducesResponseType(typeof(ResponseModel), 400)]
        public async Task<IActionResult> UpdateAsync([FromBody] UpdateApplicantHistoryRequest request)
        {
            var result = await _applicantWorkService.UpdateAsync(request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet, Route("GetSingleHistory")]
        [OpenApiOperation("Get Single History", "Single History")]
        [ProducesResponseType(typeof(ResponseModel<ApplicantHistoryResponse>), 200)]
        [ProducesResponseType(typeof(ResponseModel<ApplicantHistoryResponse>), 400)]
        public async Task<IActionResult> GetSingle([FromQuery] Guid historyId)
        {
            var result = await _applicantWorkService.GetSingleAsync(historyId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet, Route("GetApplicantHistoryList")]
        [OpenApiOperation("Get Applicant Histories By Applicant Id", "History List")]
        [ProducesResponseType(typeof(ResponseModel<CustomPagination<List<ApplicantHistoryResponse>>>), 200)]
        [ProducesResponseType(typeof(ResponseModel<CustomPagination<List<ApplicantHistoryResponse>>>), 400)]
        public async Task<IActionResult> GetByCompany([FromQuery] GetHistoryRequestList req)
        {
            var result = await _applicantWorkService.GetAllListAsync(req);
            return StatusCode(result.StatusCode, result);
        }
    }
}