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
    public class ApplicantSkillController : Controller
    {
        private readonly IApplicantSkill _applicantSkillService;

        public ApplicantSkillController(IApplicantSkill applicantSkillService)
        {
            _applicantSkillService = applicantSkillService;
        }


        [HttpPost]
        [OpenApiOperation("create applicant skill", "An endpoint for creating new skill")]
        [ProducesResponseType(typeof(ResponseModel<ApplicantSkillResponse>), 200)]
        [ProducesResponseType(typeof(ResponseModel), 400)]
        public async Task<IActionResult> CreateAsync([FromBody] ApplicantSkillRequest request)
        {
            var result = await _applicantSkillService.CreateAsync(request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete]
        [OpenApiOperation("delete applicant skill", "An endpoint for deleting  applicant skill")]
        [ProducesResponseType(typeof(ResponseModel<ApplicantSkillResponse>), 200)]
        [ProducesResponseType(typeof(ResponseModel), 400)]
        public async Task<IActionResult> DeleteAsync([FromQuery] Guid SkillId)
        {
            var result = await _applicantSkillService.DeleteAsync(SkillId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut]
        [OpenApiOperation("Update applicant skill", "An endpoint for updating  applicant skill")]
        [ProducesResponseType(typeof(ResponseModel<ApplicantSkillResponse>), 200)]
        [ProducesResponseType(typeof(ResponseModel), 400)]
        public async Task<IActionResult> UpdateAsync([FromBody] UpdateApplicantSkillRequest request)
        {
            var result = await _applicantSkillService.UpdateAsync(request);
            return StatusCode(result.StatusCode, result);
        }


        [HttpGet, Route("GetApplicantSkillList")]
        [OpenApiOperation("Get Applicant Skill list", "Skill List")]
        [ProducesResponseType(typeof(ResponseModel<List<ApplicantSkillResponse>>), 200)]
        [ProducesResponseType(typeof(ResponseModel<List<ApplicantSkillResponse>>), 400)]
        public async Task<IActionResult> GetByCompany([FromQuery] ApplicantSkillFilter filter)
        {
            var result = await _applicantSkillService.GetListAsync(filter);
            return StatusCode(result.StatusCode, result);
        }


        [HttpGet, Route("Getskillsuggestion")]
        [OpenApiOperation("Get Applicant Skill list suggestion", "Skill suggestion")]
        [ProducesResponseType(typeof(ResponseModel<List<ApplicantSkillNameSuggestion>>), 200)]
        [ProducesResponseType(typeof(ResponseModel<List<ApplicantSkillNameSuggestion>>), 400)]
        public async Task<IActionResult> GetApplicantSillSuggestions()
        {
            var result = await _applicantSkillService.GetSkillNameSuggestions();
            return StatusCode(result.StatusCode, result);
        }
    }
}