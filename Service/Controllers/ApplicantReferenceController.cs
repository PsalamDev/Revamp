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
    public class ApplicantReferenceController : Controller
    {
        private readonly IApplicantReference _applicantReferenceService;

        public ApplicantReferenceController(IApplicantReference applicantReferenceService)
        {
            _applicantReferenceService = applicantReferenceService;
        }


        [HttpPost]
        [OpenApiOperation("create applicant reference", "An endpoint for creating new applicant reference")]
        [ProducesResponseType(typeof(ResponseModel<ApplicantReferenceResponse>), 200)]
        [ProducesResponseType(typeof(ResponseModel), 400)]
        public async Task<IActionResult> CreateAsync([FromBody] ApplicantReferenceRequest request)
        {
            var result = await _applicantReferenceService.CreateAsync(request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete]
        [OpenApiOperation("delete applicant reference", "An endpoint for creating new applicant reference")]
        [ProducesResponseType(typeof(ResponseModel<ApplicantReferenceResponse>), 200)]
        [ProducesResponseType(typeof(ResponseModel), 400)]
        public async Task<IActionResult> DeleteAsync([FromQuery] Guid ReferenceId)
        {
            var result = await _applicantReferenceService.DeleteAsync(ReferenceId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut]
        [OpenApiOperation("Update applicant reference", "An endpoint for updating  applicant reference")]
        [ProducesResponseType(typeof(ResponseModel<ApplicantReferenceResponse>), 200)]
        [ProducesResponseType(typeof(ResponseModel), 400)]
        public async Task<IActionResult> CreateAsync([FromBody] UpdateApplicantReferenceRequest request)
        {
            var result = await _applicantReferenceService.UpdateAsync(request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet, Route("GetSingleReference")]
        [OpenApiOperation("Get Single Reference", "Single Reference")]
        [ProducesResponseType(typeof(ResponseModel<ApplicantReferenceResponse>), 200)]
        [ProducesResponseType(typeof(ResponseModel<ApplicantReferenceResponse>), 400)]
        public async Task<IActionResult> GetSingle([FromQuery] Guid ReferenceId)
        {
            var result = await _applicantReferenceService.GetSingleAsync(ReferenceId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet, Route("GetApplicantReferenceList")]
        [OpenApiOperation("Get Applicant Profiles By Company", "Reference List")]
        [ProducesResponseType(typeof(ResponseModel<CustomPagination<List<ApplicantReferenceListResponse>>>), 200)]
        [ProducesResponseType(typeof(ResponseModel<CustomPagination<List<ApplicantReferenceListResponse>>>), 400)]
        public async Task<IActionResult> GetByCompany([FromQuery] ReferenceListRequest req)
        {
            var result = await _applicantReferenceService.GetAllListAsync(req);
            return StatusCode(result.StatusCode, result);
        }

    }
}