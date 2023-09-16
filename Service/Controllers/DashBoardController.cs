using Core.Common.Model;
using Core.Common.Model.RecruitmentDto;
using Core.Interfaces;
using HRShared.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Service.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DashBoardController : Controller
    {
        private readonly IRecruitmentJobApplicationServices _recruitmentJobApplicationServices;
        private readonly IJobService _jobService;

        public DashBoardController(IRecruitmentJobApplicationServices recruitmentJobApplicationServices,
                                        IJobService jobService)
        {
            _recruitmentJobApplicationServices = recruitmentJobApplicationServices;
            _jobService = jobService;
        }

        [HttpGet, Route("all-hired-applicant-count")]
        [ProducesResponseType(typeof(ResponseModel<int>), 200)]
        [ProducesResponseType(typeof(ResponseModel<int>), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> FetchAllHiredApplicantsCount()
        {
            var result = await _recruitmentJobApplicationServices.FetchAllHiredApplicantsCount();
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet, Route("all-rejected-applicant-count")]
        [ProducesResponseType(typeof(ResponseModel<int>), 200)]
        [ProducesResponseType(typeof(ResponseModel<int>), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> FetchAllRejectedApplicantsCount()
        {
            var result = await _recruitmentJobApplicationServices.FetchAllRejectedOffersCount();
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet, Route("number-of-application-from-job-posted")]
        [ProducesResponseType(typeof(ResponseModel<int>), 200)]
        [ProducesResponseType(typeof(ResponseModel<int>), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> NumberOfApplicationsFromJobsPosted()
        {
            var result = await _recruitmentJobApplicationServices.NumberOfApplicationsFromJobsPosted();
            return StatusCode(result.StatusCode, result);
        }


        [HttpGet, Route("number-of-posted-jobs")]
        [ProducesResponseType(typeof(ResponseModel<int>), 200)]
        [ProducesResponseType(typeof(ResponseModel<int>), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> NumberOfPostedJobs()
        {
            var result = await _recruitmentJobApplicationServices.NumberOfJobsPosted();
            return StatusCode(result.StatusCode, result);
        }


        [HttpGet, Route("application-by-job-title")]
        [ProducesResponseType(typeof(ResponseModel<CustomPagination<List<JobApplicationListingDTO>>>), 200)]
        [ProducesResponseType(typeof(ResponseModel<CustomPagination<List<JobApplicationListingDTO>>>), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> FetchJobApplicationByRole([FromQuery] JobApplicationByJobTitle request)
        {
            var result = await _recruitmentJobApplicationServices.FetchJobApplicationByRole(request);
            return StatusCode(result.StatusCode, result);
        }


        [HttpGet, Route("job-applications")]
        [ProducesResponseType(typeof(ResponseModel<CustomPagination<List<JobApplicationDto>>>), 200)]
        [ProducesResponseType(typeof(ResponseModel<CustomPagination<List<JobApplicationDto>>>), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> FetchJobApplicationByRole([FromQuery] JobApplicationList request)
        {
            var result = await _recruitmentJobApplicationServices.FetchAllJobApplications(request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet, Route("load-applicant-score-card")]
        [ProducesResponseType(typeof(ResponseModel<EvaluationRecruitmentScoreCardDTO>), 200)]
        [ProducesResponseType(typeof(ResponseModel<EvaluationRecruitmentScoreCardDTO>), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> LoadApplicantScoreCard([FromQuery] SaveApplicantScoreCardEvaluationDto request)
        {
            var result = await _recruitmentJobApplicationServices.getApplicantScoreCard(request);
            return StatusCode(result.StatusCode, result);
        }


        [HttpPost, Route("save-applicant-score-card")]
        [ProducesResponseType(typeof(ResponseModel<string>), 200)]
        [ProducesResponseType(typeof(ResponseModel<string>), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> SaveApplicantScoreCard([FromBody] SaveApplicantScoreCardDto request)
        {
            var result = await _recruitmentJobApplicationServices.SaveApplicantScoreCard(request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet, Route("dashboard-data")]
        [ProducesResponseType(typeof(ResponseModel<DashboardChartModel>), 200)]
        [ProducesResponseType(typeof(ResponseModel<DashboardChartModel>), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> FetchDashBoardData([FromQuery] DashBoardFilter request)
        {
            var result = await _jobService.DashBoardData(request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet, Route("open-job-title-by-department")]
        [ProducesResponseType(typeof(ResponseModel<List<DashBoardListGraphData>>), 200)]
        [ProducesResponseType(typeof(ResponseModel<List<DashBoardListGraphData>>), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> OpenJobRolesByDepartment([FromQuery] string? timePeriod = null, [FromQuery] DateTime? selectedStartDate = null, [FromQuery] DateTime? selectedEndDate = null)
        {
            var result = await _jobService.OpenJobRolesByDepartment(timePeriod, selectedStartDate, selectedEndDate);
            return StatusCode(result.StatusCode, result);
        }


        [HttpGet, Route("JobsPerRole")]
        [ProducesResponseType(typeof(ResponseModel<List<DashBoardListGraphData>>), 200)]
        [ProducesResponseType(typeof(ResponseModel<List<DashBoardListGraphData>>), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> JobsPerRole([FromQuery] string? timePeriod = null, [FromQuery] DateTime? selectedStartDate = null, [FromQuery] DateTime? selectedEndDate = null)
        {
            var result = await _jobService.JobsPerRole(timePeriod, selectedStartDate, selectedEndDate);
            return StatusCode(result.StatusCode, result);
        }


        [HttpGet, Route("ApplicantDistributionByAge")]
        [ProducesResponseType(typeof(ResponseModel<List<DashBoardListGraphData>>), 200)]
        [ProducesResponseType(typeof(ResponseModel<List<DashBoardListGraphData>>), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> ApplicantDistributionByAge([FromQuery] string? timePeriod = null, [FromQuery] DateTime? selectedStartDate = null, [FromQuery] DateTime? selectedEndDate = null)
        {
            var result = await _jobService.ApplicantDistributionByAge(timePeriod);
            return StatusCode(result.StatusCode, result);
        }


        [HttpGet, Route("EmployeeTurnOver")]
        [ProducesResponseType(typeof(ResponseModel<EmployeeTurnOverDashBoardListGraphData>), 200)]
        [ProducesResponseType(typeof(ResponseModel<EmployeeTurnOverDashBoardListGraphData>), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> EmployeeTurnOver([FromQuery] string? timePeriod = null, [FromQuery] DateTime? selectedStartDate = null, [FromQuery] DateTime? selectedEndDate = null)
        {
            var result = await _jobService.EmployeeTurnOver(timePeriod, selectedStartDate, selectedEndDate);
            return StatusCode(result.StatusCode, result);
        }


        [HttpGet, Route("SourceOfApplication")]
        [ProducesResponseType(typeof(ResponseModel<List<DashBoardListGraphData>>), 200)]
        [ProducesResponseType(typeof(ResponseModel<DashBoardListGraphData>), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> SourceOfApplication([FromQuery] string? timePeriod = null, [FromQuery] DateTime? selectedStartDate = null, [FromQuery] DateTime? selectedEndDate = null)
        {
            var result = await _jobService.SourceOfApplication(timePeriod, selectedStartDate, selectedEndDate);
            return StatusCode(result.StatusCode, result);
        }


        [HttpGet, Route("DistributionBySkillSets")]
        [ProducesResponseType(typeof(ResponseModel<List<RoleSkill>>), 200)]
        [ProducesResponseType(typeof(ResponseModel<List<RoleSkill>>), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DistributionBySkillSets([FromQuery] string? timePeriod = null, [FromQuery] DateTime? selectedStartDate = null, [FromQuery] DateTime? selectedEndDate = null)
        {
            var result = await _jobService.DistributionBySkillSets(timePeriod, selectedStartDate, selectedEndDate);
            return StatusCode(result.StatusCode, result);
        }


        [HttpGet, Route("RecruitmentOverTime")]
        [ProducesResponseType(typeof(ResponseModel<List<DashBoardListGraphDataForDate>>), 200)]
        [ProducesResponseType(typeof(ResponseModel<List<DashBoardListGraphDataForDate>>), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> RecruitmentOverTime([FromQuery] string? timePeriod = null, [FromQuery] DateTime? selectedStartDate = null, [FromQuery] DateTime? selectedEndDate = null)
        {
            var result = await _jobService.RecruitmentOverTime(timePeriod, selectedStartDate, selectedEndDate);
            return StatusCode(result.StatusCode, result);
        }


        [HttpGet, Route("pendinginterview")]
        [ProducesResponseType(typeof(ResponseModel<int>), 200)]
        [ProducesResponseType(typeof(ResponseModel<int>), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Pendinginterview()
        {
            var result = await _recruitmentJobApplicationServices.Pendinginterview();
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet, Route("NumberOfApplicantInterviewed")]
        [ProducesResponseType(typeof(ResponseModel<int>), 200)]
        [ProducesResponseType(typeof(ResponseModel<int>), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> NumberOfApplicantInterviewed()
        {
            var result = await _recruitmentJobApplicationServices.NumberOfApplicantInterviewed();
            return StatusCode(result.StatusCode, result);
        }


        [HttpGet, Route("InvitedApplicants")]
        [ProducesResponseType(typeof(ResponseModel<int>), 200)]
        [ProducesResponseType(typeof(ResponseModel<int>), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> InvitedApplicants()
        {
            var result = await _recruitmentJobApplicationServices.InvitedApplicants();
            return StatusCode(result.StatusCode, result);
        }


        [HttpGet, Route("SuccessfulApplicants")]
        [ProducesResponseType(typeof(ResponseModel<List<DashBoardListGraphDataForDate>>), 200)]
        [ProducesResponseType(typeof(ResponseModel<List<DashBoardListGraphDataForDate>>), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> SuccessfulApplicants([FromQuery] string? timePeriod = null, [FromQuery] DateTime? selectedStartDate = null, [FromQuery] DateTime? selectedEndDate = null)
        {
            var result = await _recruitmentJobApplicationServices.SuccessfulApplicants(timePeriod, selectedStartDate, selectedEndDate);
            return StatusCode(result.StatusCode, result);
        }


        [HttpGet, Route("ApplicantsDropOffStages")]
        [ProducesResponseType(typeof(ResponseModel<List<DashBoardListGraphDataForDate>>), 200)]
        [ProducesResponseType(typeof(ResponseModel<List<DashBoardListGraphDataForDate>>), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> ApplicantsDropOffStages([FromQuery] string? timePeriod = null, [FromQuery] DateTime? selectedStartDate = null, [FromQuery] DateTime? selectedEndDate = null)
        {
            var result = await _recruitmentJobApplicationServices.ApplicantsDropOffStages(timePeriod, selectedStartDate, selectedEndDate);
            return StatusCode(result.StatusCode, result);
        }


        [HttpGet, Route("GetAllUpComingTasks")]
        [ProducesResponseType(typeof(ResponseModel<CustomPagination<List<ReveiwerUpComingTaskDto>>>), 200)]
        [ProducesResponseType(typeof(ResponseModel<CustomPagination<List<ReveiwerUpComingTaskDto>>>), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllUpComingTasks([FromQuery] PaginationRequest request)
        {
            var result = await _recruitmentJobApplicationServices.GetAllUpComingTasks(request);
            return StatusCode(result.StatusCode, result);
        }

    }
}