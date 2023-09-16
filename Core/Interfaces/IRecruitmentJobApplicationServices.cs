using Core.Common.Model;
using Core.Common.Model.RecruitmentDto;
using HRShared.Common;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IRecruitmentJobApplicationServices
    {
        Task<ResponseModel<EvaluationRecruitmentScoreCardDTO>> getApplicantScoreCard(SaveApplicantScoreCardEvaluationDto payload);
        Task<ResponseModel<string>> SaveApplicantScoreCard(SaveApplicantScoreCardDto payload);
        Task<ResponseModel<string>> AssignStatustoJobApplication(AssingStatusToJobApplicationDto request);
        Task<ResponseModel<CustomPagination<List<JobApplicationListingDTO>>>> FetchJobApplicationByRole(JobApplicationByJobTitle filter);
        Task<ResponseModel<CustomPagination<List<ApplicantInterviewScheduleDTO>>>> FetchJobInterviewerListByApplicationId(ApplicantInterviewScheduleFilterDTO payload);
        Task<ResponseModel<string>> SendMailToApplicant(Guid Id);
        Task<ResponseModel<string>> ShareApplicantProfileForInterview(ScheduleJobInterviewDto payload);
        Task<ResponseModel<List<InterviwerListforSharingDTO>>> GetInterviwerforSharing(Guid applicationId, Guid hirestageId, Guid subhirestageId);
        Task<ResponseModel<JobApplicationDto>> ViewJobApplicationProfileById(Guid JobApplicantId);
        Task<ResponseModel<string>> AddUpdateJobApplication(ManageJobApplicationDto payload);
        Task<ResponseModel<string>> ApplyForJob(ApplyForJobDto payload);
        Task<ResponseModel<CustomPagination<List<JobApplicationListDto>>>> FetchJobApplications(JobApplicationSearch payload);
        Task<ResponseModel<CustomPagination<List<JobApplicationDto>>>> FetchAllJobApplications(JobApplicationList payload);
        Task<ResponseModel<List<FetchApplicantDropOffStageDto>>> FetchApplicantDropOffStage(int year);
        Task<ResponseModel<CustomPagination<List<JobApplicationDto>>>> FetchAllHiredApplicants(JobApplicationList payload);
        Task<ResponseModel<CustomPagination<List<JobApplicationDto>>>> FetchAllRejectedOffers(JobApplicationList payload);
        Task<ResponseModel<List<JobApplicantionSourceDto>>> FetchJobApplicationBySource(int year);
        Task<ResponseModel<List<JobsByRoleDto>>> FetchAllJobsByRole(int year);
        Task<ResponseModel<List<JobsByDepartmentDto>>> FetchAllJobsByDepartment(int year);
        Task<ResponseModel<List<JobApplicationAgeBandDto>>> FetchJobApplicationByAgeBand(int year);
        Task<ResponseModel<List<JobRecruitmentOvertimeDto>>> GetRecruitmentOverTime(int year);
        Task<ResponseModel<List<EmployeeSkillDistributionDto>>> ListEmployeeSkillDistribution(int year);
        Task<ResponseModel<List<InvitedApplicantsListDto>>> ListOfInvitedApplicants(JobApplicationList payload);
        Task<ResponseModel<List<InterviewedApplicantsDto>>> ListOfInterviewedApplicants(JobApplicationList payload);
        Task<ResponseModel<List<ApplicantsPendingOfferListDto>>> ListOfApplicantsPendingOffer(JobApplicationList payload);
        Task<ResponseModel<List<InterviewHistoryDto>>> ListApplicantInterviewHistory(Guid applicationId, Guid applicantId);
        Task<ResponseModel<CustomPagination<List<JobApplicationUpcomingInterviews>>>> GetAllUpComingTasks(JobApplicationList payload);
        Task<ResponseModel<string>> JobApplicationApproval(JobApplicationUpdateStatus payload);
        Task<ResponseModel<string>> AddUpdateScheduleJobInterviews(JobApplicantScheduleInterview payload);
        Task<ResponseModel<List<JobApplicationDto>>> GetJobApplicationByApplicant(Guid ApplicantID);
        Task<ResponseModel<string>> DeleteJobApplication(Guid id);
        Task<ResponseModel<int>> FetchAllHiredApplicantsCount();
        Task<ResponseModel<int>> FetchAllRejectedOffersCount();
        Task<ResponseModel<int>> NumberOfApplicationsFromJobsPosted();
        Task<ResponseModel<int>> NumberOfJobsPosted();
        Task<ResponseModel<CustomPagination<List<JobListDto>>>> FetchJobApplicationByPostingStatus(JobListFilterDto request);
        Task<ResponseModel<CustomPagination<List<SupportOfficerApplication>>>> GetSupportOfficerApplication(SupportOfficerApplicationFilter filter);
        Task<ResponseModel<CustomPagination<List<ApplicantSelection>>>> GetApplicantSelection(ApplicantSelectionFilter filter);
        Task<ResponseModel<CustomPagination<List<CandidateApplicantSelection>>>> GetCandidateApplicantSelection(CandidateApplicantSelectionFilter filter);
        Task<ResponseModel<List<JobApplicationStageHistoryModel>>> JobApplicationStageHistory(Guid jobApplicationId);
        Task<List<DepartmentDataFromGRPC>> GrpcDepartmentList();
        Task<ResponseModel<int>> InvitedApplicants();
        Task<ResponseModel<int>> NumberOfApplicantInterviewed();
        Task<ResponseModel<int>> Pendinginterview();
        Task<ResponseModel<List<DashBoardListGraphData>>> SuccessfulApplicants(string? timePeriod = null, DateTime? selectedStartDate = null, DateTime? selectedEndDate = null);
        Task<ResponseModel<List<DashBoardListGraphData>>> ApplicantsDropOffStages(string? timePeriod = null, DateTime? selectedStartDate = null, DateTime? selectedEndDate = null);
        Task<ResponseModel<ApplicantQuizRecordDto>> GetApplicantQuizRecord(Guid id);
        Task<ResponseModel<CustomPagination<List<ApplicantInterviewScheduleDTO>>>> FetchUpComingJobInterviewerList(PaginationRequest payload);
        Task<ResponseModel<string>> RemoveApplicantFromShortList(RemoveShortlistedJobApplicationDto request);
        Task<ResponseModel<List<CalenderInterviewDay>>> InterviewCalender();
        Task<ResponseModel<CustomPagination<List<ReveiwerUpComingTaskDto>>>> GetAllUpComingTasks(PaginationRequest payload);
        Task<ResponseModel<DocumentResponseDTO>> DownloadJobApplicantionCV(Guid applicantionId);
        Task<ResponseModel<string>> SaveApplicantCVToCVBank(CreateApplicantCVtoCVBank payload);
        Task<ResponseModel<string>> AddApplicantobWordSerchAsync(CreateJobWordDto request);
        Task<ResponseModel<string>> SaveJobApplicantScoreCard(SaveJobApplicantScoreCardDto payload);

        Task<ResponseModel<string>> OfferLetterConfirmation(string onboardingId);

    }
}
