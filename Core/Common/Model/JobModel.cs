using Core.Interfaces;
using DocumentFormat.OpenXml.Office2010.Excel;
using HRShared.Common;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Domain.Enums.Enum;

namespace Core.Common.Model
{
    public class JobModel : BaseResponseModel
    {
        public string JobTitle { get; set; }
        public Guid DepartmentId { get; set; }

        public Guid? CountryId { get; set; }
        public Guid? StateId { get; set; }

        public string Description { get; set; }
        public string Requirements { get; set; }

        public EmploymentType EmploymentType { get; set; }
        public JobAvailablity Availability { get; set; }
        public string Experience { get; set; }
        public string MinimumEducation { get; set; }
        public string MaximumEducation { get; set; }

        public string? Currency { get; set; }
        public string? SalaryRange { get; set; }
        public DateTime? PostValidityFrom { get; set; }
        public DateTime? PostValidityTo { get; set; }
        public Guid? ScoreCardId { get; set; }
        public Guid? QuizId { get; set; }
        public Guid CompanyId { get; set; }
        public JobPostStatus JobPostStatus { get; set; }
        public JobStatus JobStatus { get; set; }


        public string? EmploymentTypeText { get; set; }
        public string? AvailabilityText { get; set; }
        public string? JobPostStatusText { get; set; }
        public string? JobStatusText { get; set; }
        public decimal? SalaryRangeFrom { get; set; }
        public decimal? SalaryRangeTo { get; set; }
        public ICollection<JobReviewerModel>? JobReviewers { get; set; }
    }

    public class JobDto
    {
        public Guid Id { get; set; }
        public string JobTitle { get; set; }
        public Guid DepartmentId { get; set; }
        public Guid? CountryId { get; set; }
        public Guid? StateId { get; set; }
        public string Description { get; set; }
        public string Requirements { get; set; }
        public EmploymentType EmploymentType { get; set; }
        public JobAvailablity Availability { get; set; }
        public string Experience { get; set; }
        public string MinimumEducation { get; set; }
        public string MaximumEducation { get; set; }
        public string? Currency { get; set; }
        public string? SalaryRange { get; set; }
        public DateTime? PostValidityFrom { get; set; }
        public DateTime? PostValidityTo { get; set; }
        public Guid? ScoreCardId { get; set; }
        public Guid? QuizId { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime DateCreated { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime? DateModified { get; set; }
        public Guid? ModifiedById { get; set; }
        public Guid CompanyId { get; set; }
        public JobPostStatus JobPostStatus { get; set; }
        public JobStatus Status { get; set; }
        public DateTime? SchedulePostDate { get; set; }
        public DateTime? DatePosted { get; set; }

        public string? EmploymentTypeText { get; set; }
        public string? AvailabilityText { get; set; }
        public string? JobPostStatusText { get; set; }
        public string? JobStatusText { get; set; }
        public decimal? SalaryRangeFrom { get; set; }
        public decimal? SalaryRangeTo { get; set; }
        public ICollection<JobReviewerModel>? JobReviewers { get; set; }
    }

    public class PostedJobModelDto
    {
        public Guid Id { get; set; }
        public string JobTitle { get; set; }
        public string DepartmentName { get; set; }

    }

    public class CreateJobDto
    {
        public string JobTitle { get; set; }
        public Guid DepartmentId { get; set; }

        public Guid CountryId { get; set; }
        public Guid StateId { get; set; }

        public string Description { get; set; }
        public string Requirements { get; set; }

        public JobAvailablity JobAvailability { get; set; }
        public EmploymentType EmploymentType { get; set; }
        public string Experience { get; set; }
        public string MinEducation { get; set; }
        public string MaxEducation { get; set; }

        public string? Currency { get; set; }

        public decimal? MinSalary { get; set; }
        public decimal? MaxSalary { get; set; }

        public Guid? ScoreCardId { get; set; }
        public Guid? QuizId { get; set; }
        public DateTime? DatePosted { get; set; }
        public DateTime? PostValidityFrom { get; set; }
        public DateTime? PostValidityTo { get; set; }
        public bool AlertSent { get; set; }
        public DateTime? SchedulePostDate { get; set; }
        public JobPostStatus JobPostStatus { get; set; }
        public ICollection<CreateJobReviewerModel>? JobReviewers { get; set; }

    }

    public class UpdateJobDto
    {
        public Guid Id { get; set; }
        public string JobTitle { get; set; }
        public Guid DepartmentId { get; set; }

        public Guid CountryId { get; set; }
        public Guid StateId { get; set; }

        public string Description { get; set; }
        public string Requirements { get; set; }

        public JobAvailablity JobAvailability { get; set; }
        public EmploymentType EmploymentType { get; set; }
        public string Experience { get; set; }
        public string MinEducation { get; set; }
        public string MaxEducation { get; set; }

        public string? Currency { get; set; }

        public decimal? MinSalary { get; set; }
        public decimal? MaxSalary { get; set; }

        public Guid? ScoreCardId { get; set; }
        public Guid? QuizId { get; set; }
        public DateTime? DatePosted { get; set; }
        public DateTime? PostValidityFrom { get; set; }
        public DateTime? PostValidityTo { get; set; }
        public bool AlertSent { get; set; }
        public DateTime? SchedulePostDate { get; set; }
        public JobPostStatus JobPostStatus { get; set; }
        public JobStatus JobStatus { get; set; }

        public ICollection<UpdateJobReviewerModel>? JobReviewers { get; set; }

    }

    public class JobReviewerModel : BaseResponseModel
    {

        public string ReviewerName { get; set; }
        public Guid ReviewerId { get; set; }
        public Guid Id { get; set; }
        public string ReviewerEmail { get; set; }
        public Guid JobId { get; set; }
        public ReviwerStatus? ReviwerStatus { get; set; }
        public string? ReviwerStatusText { get; set; }
        public string? Comment { get; set; }

    }

    public class CreateJobReviewerModel
    {
        public string ReviewerName { get; set; }
        public Guid ReviewerId { get; set; }
        public string ReviewerEmail { get; set; }
    }

    public class UpdateJobReviewerModel
    {
        public Guid Id { get; set; }
        public string ReviewerName { get; set; }
        public Guid ReviewerId { get; set; }
        public string ReviewerEmail { get; set; }
    }

    public class JobFilterDto : PaginationRequest
    {
        public JobPostStatus? JobPostStatus { get; set; }
        public string? JobTitle { get; set; }
        public Guid? DepartmentId { get; set; }
        public DateTime? DatePosted { get; set; }
        public JobStatus? Status { get; set; }
    }
    public class JobApplicantFilterDto : PaginationRequest
    {
        public string? JobTitle { get; set; }
        public string? ApplicationStatus { get; set; }
    }
    public class JobApplicantDto
    {
        public string? Fullname { get; set; }
        public string? JobTitle { get; set; }
        public DateTime DateApplied { get; set; }
        public string ApplicationStatus { get; set; }

    }

    public class JobApplicationListByJobTitleDto
    {
        public Guid ApplicationId { get; set; }
        public string JobTitle { get; set; }
        public string Fullname { get; set; }
        public string Email { get; set; }
        public DateTime DateApplied { get; set; }
        public long EvaluationScore { get; set; }
        public long QuizScore { get; set; }

    }
    public class JobApplicationByJobTitle : PaginationRequest
    {
        public string? JobTitle { get; set; }
    }

    public class JobPostReviewDto
    {
        public Guid Id { get; set; }
        public Guid JobReviwerId { get; set; }
        public Guid JobId { get; set; }
        public string? JobTitle { get; set; }
        public string? CreatedBy { get; set; }
        public string? FormatDatePosted { get; set; }
        public string? FormatTimePosted { get; set; }
        public ReviwerStatus? ReviwerStatus { get; set; }
        public string? ReviwerStatusText { get; set; }
        public DateTime DatePosted { get; set; }
        public DateTime TimePosted { get; set; }
        public DateTime CreatedOn { get; set; }

    }


    public class JobPostReviewfilterDto : PaginationRequest
    {
        public ReviwerStatus ReviwerStatus { get; set; }
    }

    public class CreateJobReviwerComent
    {
        public Guid JobId { get; set; }
        public Guid JobReviwerId { get; set; }
        public string Comment { get; set; }
    }




}
