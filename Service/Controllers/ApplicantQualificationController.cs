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
    public class ApplicantQualificationController : BaseController
    {
        private readonly IApplicantQualification _applicantQualificationService;
        public ApplicantQualificationController(IApplicantQualification applicantQualificationService)
        {
            _applicantQualificationService = applicantQualificationService;
        }

        [HttpPost]
        [OpenApiOperation("create applicant qualification history", "An endpoint for creating new applicant qualification history")]
        [ProducesResponseType(typeof(ResponseModel<ApplicantQualificationResponse>), 200)]
        [ProducesResponseType(typeof(ResponseModel), 400)]
        public async Task<IActionResult> CreateAsync([FromBody] ApplicantQualificationRequest request)
        {
            var result = await _applicantQualificationService.CreateAsync(request);
            return StatusCode(result.StatusCode, result);
        }


        [HttpPut]
        [OpenApiOperation("Update applicant qualifications", "An endpoint for updating  applicants qualifications")]
        [ProducesResponseType(typeof(ResponseModel<ApplicantQualificationResponse>), 200)]
        [ProducesResponseType(typeof(ResponseModel), 400)]
        public async Task<IActionResult> UpdateAsync([FromBody] UpdateApplicantQualificationRequest request)
        {
            var result = await _applicantQualificationService.UpdateAsync(request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete]
        [OpenApiOperation("delete applicant qualification history", "An endpoint for delete applicant qualification")]
        [ProducesResponseType(typeof(ResponseModel<ApplicantQualificationResponse>), 200)]
        [ProducesResponseType(typeof(ResponseModel), 400)]
        public async Task<IActionResult> DeleteHistory([FromBody] Guid Id)
        {
            var result = await _applicantQualificationService.DeleteAsync(Id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet, Route("GetSingleQualificationHistory")]
        [OpenApiOperation("Get single qualification history", "Single Applicant Qualification history")]
        [ProducesResponseType(typeof(ResponseModel<ApplicantQualificationResponse>), 200)]
        [ProducesResponseType(typeof(ResponseModel<ApplicantQualificationResponse>), 400)]
        public async Task<IActionResult> GetSingle([FromQuery] Guid Id)
        {
            var result = await _applicantQualificationService.GetSingleAsync(Id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet, Route("GetApplicantHistories")]
        [OpenApiOperation("Get Applicant History List", "History List")]
        [ProducesResponseType(typeof(ResponseModel<CustomPagination<List<ApplicantProfileListResponse>>>), 200)]
        [ProducesResponseType(typeof(ResponseModel<CustomPagination<List<ApplicantProfileListResponse>>>), 400)]
        public async Task<IActionResult> GetListById([FromQuery] QualificationListRequest req)
        {
            var result = await _applicantQualificationService.GetAllListAsync(req);
            return StatusCode(result.StatusCode, result);
        }

    }
}