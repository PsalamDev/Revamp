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
    public class ApplicantDocumentController : ControllerBase
    {
        private readonly IApplicantDocumentService _applicantDocService;

        public ApplicantDocumentController(IApplicantDocumentService applicantDocService)
        {
            _applicantDocService = applicantDocService;
        }


        [HttpPost]
        [OpenApiOperation("add applicants documents", "An endpoint for adding new applicant documents")]
        [ProducesResponseType(typeof(ResponseModel<ApplicantDocumentResponse>), 200)]
        [ProducesResponseType(typeof(ResponseModel), 400)]
        public async Task<IActionResult> CreateAsync([FromForm] ApplicantDocumentRequest request)
        {
            var result = await _applicantDocService.CreateAsync(request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete]
        [OpenApiOperation("delete applicant documents", "An endpoint for deleting  applicant documents")]
        [ProducesResponseType(typeof(ResponseModel<bool>), 200)]
        [ProducesResponseType(typeof(ResponseModel), 400)]
        public async Task<IActionResult> DeleteAsync([FromQuery] Guid documentId)
        {
            var result = await _applicantDocService.DeleteAsync(documentId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut]
        [OpenApiOperation("Update applicant documents", "An endpoint for updating  applicants document")]
        [ProducesResponseType(typeof(ResponseModel<ApplicantReferenceResponse>), 200)]
        [ProducesResponseType(typeof(ResponseModel), 400)]
        public async Task<IActionResult> UpdateAsync([FromForm] UpdateDocumentRequest request)
        {
            var result = await _applicantDocService.UpdateAsync(request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet, Route("GetSingleDocument")]
        [OpenApiOperation("Get Single Document", "Single Document")]
        [ProducesResponseType(typeof(ResponseModel<ApplicantReferenceResponse>), 200)]
        [ProducesResponseType(typeof(ResponseModel<ApplicantReferenceResponse>), 400)]
        public async Task<IActionResult> GetSingle([FromQuery] Guid documentId)
        {
            var result = await _applicantDocService.GetSingleAsync(documentId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet, Route("GetApplicantDocumentList")]
        [OpenApiOperation("Get Applicant Documents By Applicant Id", "Document List")]
        [ProducesResponseType(typeof(ResponseModel<CustomPagination<List<ApplicantReferenceListResponse>>>), 200)]
        [ProducesResponseType(typeof(ResponseModel<CustomPagination<List<ApplicantReferenceListResponse>>>), 400)]
        public async Task<IActionResult> GetByCompany([FromQuery] DocumentListRequest req)
        {
            var result = await _applicantDocService.GetAllListAsync(req);
            return StatusCode(result.StatusCode, result);
        }
    }
}