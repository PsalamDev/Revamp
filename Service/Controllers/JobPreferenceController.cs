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
    public class JobPreferenceController : ControllerBase
    {
        private readonly IJobPreferenceService _jobPreferenceService;

        public JobPreferenceController(IJobPreferenceService jobPreferenceService)
        {
            _jobPreferenceService = jobPreferenceService;
        }

        [HttpPost, Route("add-job-preference")]
        [ProducesResponseType(typeof(ResponseModel<JobPreferenceModel>), 200)]
        [ProducesResponseType(typeof(ResponseModel), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateAsync([FromBody] CreateJobPreferenceDto request)
        {

            var result = await _jobPreferenceService.CreateAsync(request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut, Route("update-job-preference")]
        [ProducesResponseType(typeof(ResponseModel<JobPreferenceModel>), 200)]
        [ProducesResponseType(typeof(ResponseModel), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateAsync([FromBody] UpdateJobPreferencDto request)
        {
            var result = await _jobPreferenceService.UpdateAsync(request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet, Route("load-job-preferences")]
        [ProducesResponseType(typeof(ResponseModel<CustomPagination<List<JobPreferenceDto>>>), 200)]
        [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAll([FromQuery] JobPreferenceFilter filter)
        {

            var result = await _jobPreferenceService.GetAllAsync(filter);

            return StatusCode(result.StatusCode, result);
        }

        [HttpGet, Route("load-job-preference")]
        [ProducesResponseType(typeof(ResponseModel<List<JobPreferenceDto>>), 200)]
        [ProducesResponseType(typeof(ResponseModel<List<JobPreferenceDto>>), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> LoadScoreCard([FromQuery] Guid id)
        {
            var result = await _jobPreferenceService.GetSingleAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete, Route("delete-job-preference")]
        [ProducesResponseType(typeof(ResponseModel<bool>), 200)]
        [ProducesResponseType(typeof(ResponseModel<bool>), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteStageAsync([FromQuery] Guid id)
        {

            var result = await _jobPreferenceService.DeleteAsync(id);
            return StatusCode(result.StatusCode, result);
        }

    }
}