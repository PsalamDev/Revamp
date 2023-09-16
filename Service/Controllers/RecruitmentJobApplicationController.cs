using HRShared.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using Domain.Entities;
using Domain.Interfaces;
using Core.Interfaces;
using Core.Common.Model.RecruitmentDto;
using Core.Common.Model;

namespace Service.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RecruitmentJobApplicationController : Controller
    {
        private readonly IRecruitmentJobApplicationServices _recruitmentJobApplicationServices;

        public RecruitmentJobApplicationController(IRecruitmentJobApplicationServices recruitmentJobApplicationServices)
        {
            _recruitmentJobApplicationServices = recruitmentJobApplicationServices;
        }

        [HttpGet, Route("load-job-appliant-by-jobTitle")]
        [ProducesResponseType(typeof(CustomPagination<List<JobApplicationListingDTO>>), 200)]
        [ProducesResponseType(typeof(CustomPagination<List<JobApplicationListingDTO>>), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> LoadJobApplicatByRole([FromQuery] JobApplicationByJobTitle request)
        {
            var result = await _recruitmentJobApplicationServices.FetchJobApplicationByRole(request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost, Route("recruitment-status-action")]
        [ProducesResponseType(typeof(ResponseModel<string>), 200)]
        [ProducesResponseType(typeof(ResponseModel<string>), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> RecruitmentAssingStatusAction([FromBody] AssingStatusToJobApplicationDto request)
        {
            var result = await _recruitmentJobApplicationServices.AssignStatustoJobApplication(request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet, Route("job-interviewer-list-by-applicationId")]
        [ProducesResponseType(typeof(ResponseModel<CustomPagination<List<ApplicantInterviewScheduleDTO>>>), 200)]
        [ProducesResponseType(typeof(ResponseModel<CustomPagination<List<ApplicantInterviewScheduleDTO>>>), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> JobInterviewerListByApplicationId([FromQuery] ApplicantInterviewScheduleFilterDTO request)
        {
            var result = await _recruitmentJobApplicationServices.FetchJobInterviewerListByApplicationId(request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet, Route("job-application-by-job-title")]
        [ProducesResponseType(typeof(ResponseModel<CustomPagination<List<JobApplicationListingDTO>>>), 200)]
        [ProducesResponseType(typeof(ResponseModel<CustomPagination<List<JobApplicationListingDTO>>>), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetJobApplicationByJobTitle([FromQuery] JobApplicationByJobTitle request)
        {
            var result = await _recruitmentJobApplicationServices.FetchJobApplicationByRole(request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost]
        [Route("add-update-schedule-job-interviews")]
        [ProducesResponseType(typeof(ResponseModel<string>), 200)]
        [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> AddUpdateScheduleJobInterviews([FromBody] JobApplicantScheduleInterview request)
        {
            var result = await _recruitmentJobApplicationServices.AddUpdateScheduleJobInterviews(request);
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

        [HttpGet, Route("get-applicant-score-card")]
        [ProducesResponseType(typeof(ResponseModel<EvaluationRecruitmentScoreCardDTO>), 200)]
        [ProducesResponseType(typeof(ResponseModel<EvaluationRecruitmentScoreCardDTO>), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetApplicantScoreCard([FromQuery] SaveApplicantScoreCardEvaluationDto request)
        {
            var result = await _recruitmentJobApplicationServices.getApplicantScoreCard(request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost, Route("apply-for-job")]
        [ProducesResponseType(typeof(ResponseModel<string>), 200)]
        [ProducesResponseType(typeof(ResponseModel<string>), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> ApplyForJob([FromForm] ApplyForJobDto request)
        {
            var result = await _recruitmentJobApplicationServices.ApplyForJob(request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet, Route("job-applications")]
        [ProducesResponseType(typeof(ResponseModel<JobApplicationListDto>), 200)]
        [ProducesResponseType(typeof(ResponseModel<JobApplicationListDto>), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> FetchJobApplications([FromQuery] JobApplicationSearch request)
        {
            var result = await _recruitmentJobApplicationServices.FetchJobApplications(request);
            return StatusCode(result.StatusCode, result);
        }


        [HttpGet, Route("ApplicantDropOffStage")]
        [ProducesResponseType(typeof(ResponseModel<List<FetchApplicantDropOffStageDto>>), 200)]
        [ProducesResponseType(typeof(ResponseModel<List<FetchApplicantDropOffStageDto>>), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> FetchApplicantDropOffStage([FromQuery] int year)
        {
            var result = await _recruitmentJobApplicationServices.FetchApplicantDropOffStage(year);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet, Route("job-applications-by-post-status")]
        [ProducesResponseType(typeof(ResponseModel<CustomPagination<List<JobListDto>>>), 200)]
        [ProducesResponseType(typeof(ResponseModel<CustomPagination<List<JobListDto>>>), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> FetchJobApplicationByPostingStatus([FromQuery] JobListFilterDto request)
        {
            var result = await _recruitmentJobApplicationServices.FetchJobApplicationByPostingStatus(request);
            return StatusCode(result.StatusCode, result);
        }


        [HttpGet, Route("all-hired-applicant")]
        [ProducesResponseType(typeof(ResponseModel<CustomPagination<List<JobApplicationDto>>>), 200)]
        [ProducesResponseType(typeof(ResponseModel<CustomPagination<List<JobApplicationDto>>>), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> FetchAllHiredApplicants([FromQuery] JobApplicationList request)
        {
            var result = await _recruitmentJobApplicationServices.FetchAllHiredApplicants(request);
            return StatusCode(result.StatusCode, result);
        }


        [HttpPost, Route("job-application-approval")]
        [ProducesResponseType(typeof(ResponseModel<string>), 200)]
        [ProducesResponseType(typeof(ResponseModel<string>), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> JobApplicationApproval([FromForm] JobApplicationUpdateStatus request)
        {
            var result = await _recruitmentJobApplicationServices.JobApplicationApproval(request);
            return StatusCode(result.StatusCode, result);
        }


        [HttpGet, Route("application-history")]
        [ProducesResponseType(typeof(ResponseModel<List<InterviewHistoryDto>>), 200)]
        [ProducesResponseType(typeof(ResponseModel<List<InterviewHistoryDto>>), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> ApplicantInterviewHistory([FromQuery] Guid applicationId, [FromQuery] Guid applicantId)
        {
            var result = await _recruitmentJobApplicationServices.ListApplicantInterviewHistory(applicationId, applicantId);
            return StatusCode(result.StatusCode, result);
        }


        [HttpGet, Route("support-officer-application")]
        [ProducesResponseType(typeof(ResponseModel<CustomPagination<List<SupportOfficerApplication>>>), 200)]
        [ProducesResponseType(typeof(ResponseModel<CustomPagination<List<SupportOfficerApplication>>>), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> LoadSupportOfficerApplication([FromQuery] SupportOfficerApplicationFilter filter)
        {
            var result = await _recruitmentJobApplicationServices.GetSupportOfficerApplication(filter);
            return StatusCode(result.StatusCode, result);
        }


        [HttpGet, Route("applicant-selection")]
        [ProducesResponseType(typeof(ResponseModel<CustomPagination<List<ApplicantSelection>>>), 200)]
        [ProducesResponseType(typeof(ResponseModel<CustomPagination<List<ApplicantSelection>>>), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetApplicantSelection([FromQuery] ApplicantSelectionFilter filter)
        {
            var result = await _recruitmentJobApplicationServices.GetApplicantSelection(filter);
            return StatusCode(result.StatusCode, result);
        }


        [HttpGet, Route("candidate-applicant-selection")]
        [ProducesResponseType(typeof(ResponseModel<CustomPagination<List<CandidateApplicantSelection>>>), 200)]
        [ProducesResponseType(typeof(ResponseModel<CustomPagination<List<CandidateApplicantSelection>>>), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetCandidateApplicantSelection([FromQuery] CandidateApplicantSelectionFilter filter)
        {
            var result = await _recruitmentJobApplicationServices.GetCandidateApplicantSelection(filter);
            return StatusCode(result.StatusCode, result);
        }


        [HttpGet, Route("job-application-status-histories")]
        [ProducesResponseType(typeof(ResponseModel<List<JobApplicationStageHistoryModel>>), 200)]
        [ProducesResponseType(typeof(ResponseModel<List<JobApplicationStageHistoryModel>>), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> LoadJobApplicationStageHistory([FromQuery] Guid jobApplicationId)
        {
            var result = await _recruitmentJobApplicationServices.JobApplicationStageHistory(jobApplicationId);
            return StatusCode(result.StatusCode, result);
        }


        [HttpPost, Route("share-application-profile-for-interview")]
        [ProducesResponseType(typeof(ResponseModel<string>), 200)]
        [ProducesResponseType(typeof(ResponseModel<string>), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> ShareApplicantProfileForInterviewAsync([FromQuery] ScheduleJobInterviewDto payload)
        {
            var result = await _recruitmentJobApplicationServices.ShareApplicantProfileForInterview(payload);
            return StatusCode(result.StatusCode, result);
        }


        [HttpGet("GetAllUpComingTasks")]
        [OpenApiOperation("Get All UpComing Tasks", "Get All UpComing Tasks")]
        [ProducesResponseType(typeof(ResponseModel<CustomPagination<List<JobApplicationUpcomingInterviews>>>), 200)]
        [ProducesResponseType(typeof(ResponseModel<CustomPagination<List<JobApplicationUpcomingInterviews>>>), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllUpComingTasks([FromQuery] JobApplicationList request)
        {
            var result = await _recruitmentJobApplicationServices.GetAllUpComingTasks(request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("GetSupportOfficerApplication")]
        [OpenApiOperation("Get All Support Officer Application", "Get All Support Officer Application")]
        [ProducesResponseType(typeof(ResponseModel<CustomPagination<List<SupportOfficerApplication>>>), 200)]
        [ProducesResponseType(typeof(ResponseModel<CustomPagination<List<SupportOfficerApplication>>>), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetSupportOfficerApplication([FromQuery] SupportOfficerApplicationFilter request)
        {
            var result = await _recruitmentJobApplicationServices.GetSupportOfficerApplication(request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("GetApplicantQuizByQuizId")]
        [OpenApiOperation("Get Applicant Quiz Detail", "Get Applicant Quiz Detail")]
        [ProducesResponseType(typeof(ResponseModel<ApplicantQuizRecordDto>), 200)]
        [ProducesResponseType(typeof(ResponseModel<ApplicantQuizRecordDto>), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetApplicantQuizByQuizId([FromQuery] Guid request)
        {
            var result = await _recruitmentJobApplicationServices.GetApplicantQuizRecord(request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("FetchUpComingJobInterview")]
        [OpenApiOperation("Get All UpComing Job Interview", "Get All UpComing Job Interview")]
        [ProducesResponseType(typeof(ResponseModel<CustomPagination<List<JobApplicationUpcomingInterviews>>>), 200)]
        [ProducesResponseType(typeof(ResponseModel<CustomPagination<List<JobApplicationUpcomingInterviews>>>), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> FetchUpComingJobInterviewerList([FromQuery] PaginationRequest request)
        {
            var result = await _recruitmentJobApplicationServices.FetchUpComingJobInterviewerList(request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("CalenderInterview")]
        [OpenApiOperation("calendars of the days with interviews scheduled", "calendars of the days with interviews scheduled")]
        [ProducesResponseType(typeof(ResponseModel<List<CalenderInterviewDay>>), 200)]
        [ProducesResponseType(typeof(ResponseModel<List<CalenderInterviewDay>>), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> InterviewCalender()
        {
            var result = await _recruitmentJobApplicationServices.InterviewCalender();
            return StatusCode(result.StatusCode, result);
        }


        [Authorize]
        [HttpPost("SendMailToApplicant")]
        [OpenApiOperation("email the applicant directly", "An endpoint for sending email to the applicant directly")]
        [ProducesResponseType(typeof(ResponseModel<ApplicantProfileResponse>), 200)]
        [ProducesResponseType(typeof(ResponseModel), 400)]
        public async Task<IActionResult> SendMailToApplicantAsync([FromQuery] Guid id)
        {
            var result = await _recruitmentJobApplicationServices.SendMailToApplicant(id);
            return StatusCode(result.StatusCode, result);
        }


        [Authorize]
        [HttpGet, Route("GetApplicantionCV")]
        [OpenApiOperation("Get Applicantion CV", "Applicantion CV")]
        [ProducesResponseType(typeof(ResponseModel<List<DocumentResponseDTO>>), 200)]
        [ProducesResponseType(typeof(ResponseModel<List<DocumentResponseDTO>>), 400)]
        public async Task<IActionResult> GetApplicantCV([FromQuery] Guid applicantId)
        {
            var result = await _recruitmentJobApplicationServices.DownloadJobApplicantionCV(applicantId);
            return StatusCode(result.StatusCode, result);
        }



        [HttpPost, Route("save-to-cv-bank")]
        [ProducesResponseType(typeof(ResponseModel<string>), 200)]
        [ProducesResponseType(typeof(ResponseModel<string>), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> SaveApplicantCVToCVBank([FromForm] CreateApplicantCVtoCVBank request)
        {
            var result = await _recruitmentJobApplicationServices.SaveApplicantCVToCVBank(request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost, Route("removefromshortlist")]
        [ProducesResponseType(typeof(ResponseModel<string>), 200)]
        [ProducesResponseType(typeof(ResponseModel<string>), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> RemoveApplicantFromShortListAsync([FromBody] RemoveShortlistedJobApplicationDto request)
        {
            var result = await _recruitmentJobApplicationServices.RemoveApplicantFromShortList(request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost, Route("add-job-word")]
        [ProducesResponseType(typeof(ResponseModel<string>), 200)]
        [ProducesResponseType(typeof(ResponseModel), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> AddJobWordAsync([FromBody] CreateJobWordDto request)
        {

            var result = await _recruitmentJobApplicationServices.AddApplicantobWordSerchAsync(request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost, Route("save-job-applicant-score-card")]
        [ProducesResponseType(typeof(ResponseModel<string>), 200)]
        [ProducesResponseType(typeof(ResponseModel<string>), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> SaveJobApplicantScoreCard([FromBody] SaveJobApplicantScoreCardDto request)
        {
            var result = await _recruitmentJobApplicationServices.SaveJobApplicantScoreCard(request);
            return StatusCode(result.StatusCode, result);
        }
    }
}