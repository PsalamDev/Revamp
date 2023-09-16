using Core.Common.Model;
using Core.Interfaces;
using Domain.Shared;
using HRShared.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicantController : BaseController
    {
        private readonly IApplicantProfile _applicantService;
        public ApplicantController(IApplicantProfile applicantService)
        {
            _applicantService = applicantService;
        }


        [HttpPost]
        [OpenApiOperation("Onboard applicant", "An endpoint for creating new applicant profile")]
        [ProducesResponseType(typeof(ResponseModel<ApplicantProfileResponse>), 200)]
        [ProducesResponseType(typeof(ResponseModel), 400)]
        public async Task<IActionResult> CreateAsync([FromBody] ApplicantProfileRequest request)
        {
            var result = await _applicantService.CreateAsync(request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut]
        [OpenApiOperation("Update applicant profile", "An endpoint for updating  applicant profile")]
        [ProducesResponseType(typeof(ResponseModel<ApplicantProfileResponse>), 200)]
        [ProducesResponseType(typeof(ResponseModel), 400)]
        public async Task<IActionResult> UpdateAsync([FromBody] UpdateApplicantProfileRequest request)
        {
            var result = await _applicantService.UpdateAsync(request);
            return StatusCode(result.StatusCode, result);
        }

        [Authorize]
        [HttpPut, Route("UpdateImage")]
        [OpenApiOperation("Update applicant profile image", "An endpoint for updating applicant profile image")]
        [ProducesResponseType(typeof(ResponseModel<ApplicantProfileResponse>), 200)]
        [ProducesResponseType(typeof(ResponseModel), 400)]
        public async Task<IActionResult> UpdateImageAsync([FromForm] ApplicantProfileImage request)
        {
            var result = await _applicantService.UploadApplicantProfileImage(request);
            return StatusCode(result.StatusCode, result);
        }

        [Authorize]
        [HttpGet, Route("GetApplicantProfile")]
        [OpenApiOperation("Get Applicant Profile", "Single Applicant Profile")]
        [ProducesResponseType(typeof(ResponseModel<ApplicantProfileResponse>), 200)]
        [ProducesResponseType(typeof(ResponseModel<ApplicantProfileResponse>), 400)]
        public async Task<IActionResult> GetSingle([FromQuery] Guid ApplicantId)
        {
            var result = await _applicantService.GetSingleAsync(ApplicantId);
            return StatusCode(result.StatusCode, result);
        }

        [Authorize]
        [HttpGet, Route("GetApplicantProfileEmail")]
        [OpenApiOperation("Get Applicant Profile by Email", "Single Applicant Profile By Email")]
        [ProducesResponseType(typeof(ResponseModel<ApplicantProfileResponse>), 200)]
        [ProducesResponseType(typeof(ResponseModel<ApplicantProfileResponse>), 400)]
        public async Task<IActionResult> GetSingle([FromQuery] string ApplicantEmail)
        {
            var result = await _applicantService.GetSingleByEmailAsync(ApplicantEmail);
            return StatusCode(result.StatusCode, result);
        }

        [Authorize]
        [HttpGet, Route("GetApplicantList")]
        [OpenApiOperation("Get Applicant Profiles By Company", "Profile List")]
        [ProducesResponseType(typeof(ResponseModel<CustomPagination<List<ApplicantProfileListResponse>>>), 200)]
        [ProducesResponseType(typeof(ResponseModel<CustomPagination<List<ApplicantProfileListResponse>>>), 400)]
        public async Task<IActionResult> GetByCompany([FromQuery] ProfileListRequest req)
        {
            var result = await _applicantService.GetAllListAsync(req);
            return StatusCode(result.StatusCode, result);
        }

        [Authorize]
        [HttpGet, Route("GetApplicantCV")]
        [OpenApiOperation("Get Applicant CV", "Applicant CV")]
        [ProducesResponseType(typeof(ResponseModel<List<DocumentResponseDTO>>), 200)]
        [ProducesResponseType(typeof(ResponseModel<List<DocumentResponseDTO>>), 400)]
        public async Task<IActionResult> GetApplicantCV([FromQuery] Guid applicantId)
        {
            var result = await _applicantService.DownloadApplicantCV(applicantId);
            return StatusCode(result.StatusCode, result);
        }




        [Authorize]
        [HttpPut, Route("UploadCV")]
        [OpenApiOperation("Update applicant CV", "An endpoint for updating applicant CV")]
        [ProducesResponseType(typeof(ResponseModel<ApplicantProfileResponse>), 200)]
        [ProducesResponseType(typeof(ResponseModel), 400)]
        public async Task<IActionResult> UpdateApplicantCVAsync([FromForm] ApplicantProfileImage request)
        {
            var result = await _applicantService.UploadApplicantProfileImage(request);
            return StatusCode(result.StatusCode, result);
        }



        [Authorize]
        [HttpGet, Route("load-applicant-keywords")]
        [ProducesResponseType(typeof(ResponseModel<List<JobWordDto>>), 200)]
        [ProducesResponseType(typeof(ResponseModel<List<JobWordDto>>), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> LoadApplicantKeywordAsync([FromQuery] Guid applicantId)
        {
            var result = await _applicantService.LoadApplicantKeyword(applicantId);
            return StatusCode(result.StatusCode, result);
        }

        [Authorize]
        [HttpDelete, Route("delete-applicant-keyword")]
        [ProducesResponseType(typeof(ResponseModel<bool>), 200)]
        [ProducesResponseType(typeof(ResponseModel<bool>), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteRecruitmentFocusAreaAsync([FromQuery] Guid id)
        {
            var result = await _applicantService.DeleteApplicanKeywordAsync(id);
            return StatusCode(result.StatusCode, result);
        }

    }
}