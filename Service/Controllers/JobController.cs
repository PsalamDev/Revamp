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
    public class JobController : ControllerBase
    {
        private readonly IJobService _JobService;
        public JobController(IJobService jobService)
        {
            _JobService = jobService;
        }

        [HttpPost, Route("add-job")]
        [ProducesResponseType(typeof(ResponseModel<JobModel>), 200)]
        [ProducesResponseType(typeof(ResponseModel<JobModel>), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateAsync([FromBody] CreateJobDto request)
        {

            var result = await _JobService.CreateAsync(request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut, Route("update-job")]
        [ProducesResponseType(typeof(ResponseModel<JobModel>), 200)]
        [ProducesResponseType(typeof(ResponseModel<JobModel>), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateAsync([FromBody] UpdateJobDto request)
        {
            var result = await _JobService.UpdateAsync(request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet, Route("load-jobs")]
        [ProducesResponseType(typeof(ResponseModel<CustomPagination<List<JobDto>>>), 200)]
        [ProducesResponseType(typeof(ResponseModel<CustomPagination<List<JobDto>>>), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAll([FromQuery] JobFilterDto filter)
        {

            var result = await _JobService.GetAllAsync(filter);

            return StatusCode(result.StatusCode, result);
        }

        [HttpGet, Route("load-job")]
        [ProducesResponseType(typeof(ResponseModel<JobDto>), 200)]
        [ProducesResponseType(typeof(ResponseModel<JobDto>), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> LoadJob([FromQuery] Guid id)
        {

            var result = await _JobService.GetSingleAsync(id);

            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete, Route("delete-job")]
        [ProducesResponseType(typeof(ResponseModel<bool>), 200)]
        [ProducesResponseType(typeof(ResponseModel<bool>), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteStageAsync([FromQuery] Guid id)
        {

            var result = await _JobService.DeleteAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet, Route("load-job-post-review")]
        [ProducesResponseType(typeof(ResponseModel<CustomPagination<List<JobPostReviewDto>>>), 200)]
        [ProducesResponseType(typeof(ResponseModel<CustomPagination<List<JobPostReviewDto>>>), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAll([FromQuery] JobPostReviewfilterDto filter)
        {

            var result = await _JobService.JobPostReview(filter);

            return StatusCode(result.StatusCode, result);
        }

        [HttpPut, Route("create-reviewer-comment")]
        [ProducesResponseType(typeof(ResponseModel<string>), 200)]
        [ProducesResponseType(typeof(ResponseModel<string>), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> ReviewerComent([FromBody] CreateJobReviwerComent request)
        {
            var result = await _JobService.JobPostReviewComment(request);
            return StatusCode(result.StatusCode, result);
        }
    }
}
