using HRShared.Common;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using static Domain.Enums.Enum;

namespace Core.Common.Model.RecruitmentDto
{
    public class JobApplicantDto
    {
        public Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public int TitleId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string OtherName { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime? DateCreated { get; set; }
        public Guid? StateOfResidentId { get; set; }
        public Guid? NationalityId { get; set; }
        //public string Skills { get; set; }
        public int? YearOfExperience { get; set; }
        public DateTime? LastUpdate { get; set; }
        public string Password { get; set; }
        public int? Gender { get; set; }
        public string Title { get; set; }
        public int? NYSCStatus { get; set; }
        public string WorkAchievements { get; set; }
        public string CVName { get; set; }
        public string CVMimeType { get; set; }
        public string DesiredSalaryCurrency { get; set; }
        public decimal? DesiredSalaryAnnum { get; set; }
        public bool DesiredSalaryNegotiable { get; set; }
        public bool WillingToRelocate { get; set; }
        public string PhotoName { get; set; }
        public string PhotoMimeType { get; set; }
        public bool? CompleteProfile { get; set; }
        public int Age { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public string Address { get; set; }
        public string JobSource { get; set; }
        public string AgeBand { get; set; }
        public decimal ScoreCardValue { get; set; }
        //public string SelectedEducation { get; set; }
        //public string SelectedWorkExperience { get; set; }
        //public string SelectedReference { get; set; }
        public string Education { get; set; }
        public string WorkExperience { get; set; }
        public string Reference { get; set; }
        public string TempRef { get; set; }
        public string LinkedIn { get; set; }
        public string Website { get; set; }
        public string Instagram { get; set; }
        public string Twitter { get; set; }
        public int? SkillId { get; set; }
        public List<JobApplicantSkillDto> Skill { get; set; }
        public List<JobApplicationDto> JobApplications { get; set; }
        public List<JobApplicantEducationDto> JobApplicantEducations { get; set; }
        public List<JobApplicantReferenceDto> JobApplicantReferences { get; set; }
        public List<JobApplicantWorkExperienceDto> JobApplicantWorkExperiences { get; set; }
        public List<ApplicantDocumentDto> ApplicantDocuments { get; set; }
    }

    public class ApplicantDocumentDto
    {
        public Guid ApplicantId { get; set; }
        public string FileType { get; set; }
        public string FileName { get; set; }
        public string Comment { get; set; }
        public string FileUrl { get; set; }

        public Guid DocuemntType { get; set; }

        public string DocumentTitle { get; set; }
    }

    public class JobApplicantWorkExperienceDto
    {
        public Guid ApplicantId { get; set; }
        public string CompanyName { get; set; }
        public string JobTitle { get; set; }
        public string Department { get; set; }
        public string Grade { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public bool IsCurrent { get; set; }
        public string? JobDescription { get; set; }
    }

    public class JobApplicantReferenceDto
    {
        public Guid CompanyId { get; set; }
        public string FullName { get; set; }
        public string Profession { get; set; }
        public string PlaceOfWork { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public Guid JobApplicantId { get; set; }
    }

    public class JobApplicantEducationDto
    {
        public Guid ApplicantId { get; set; }
        public Guid QualificationType { get; set; }
        public string QualificationDegreeName { get; set; }
        public string ProgramTypeName { get; set; }
        public string InstitutionName { get; set; }
        public string CGPA { get; set; }
        public Guid? CourseId { get; set; }
        public string CourseName { get; set; }
        public Guid? GradeId { get; set; }
        public string GradeName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsOngoing { get; set; }
    }

    public class JobApplicantSkillDto
    {
        public Guid CompanyId { get; set; }
        public Guid ApplicantsId { get; set; }
        public string Category { get; set; }
        public Guid Skill { get; set; }
        public string SkillName { get; set; }
        public DateTime StartDate { get; set; }
        public int NumberOfExpMonths { get; set; }
    }

    public class JobApplicantsDto
    {
        public long ID { get; set; }
        public long? UserId { get; set; }
        public int TitleId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string OtherName { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public DateTime? BirthDate { get; set; }
        public long? StateOfResidentId { get; set; }
        public long? NationalityId { get; set; }
        public int? YearOfExperience { get; set; }
        public DateTime? LastUpdate { get; set; }
        public string Password { get; set; }
        public int? Gender { get; set; }
        public string Title { get; set; }
        public int? NYSCStatus { get; set; }
        //public string WorkAchievements { get; set; }
        //public string CVName { get; set; }
        //public string CVMimeType { get; set; }
        public string DesiredSalaryCurrency { get; set; }
        public decimal? DesiredSalaryAnnum { get; set; }
        public bool DesiredSalaryNegotiable { get; set; }
        public bool WillingToRelocate { get; set; }
        //public string PhotoName { get; set; }
        //public string PhotoMimeType { get; set; }
        public bool? CompleteProfile { get; set; }
        //public int Age { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        //public string UserName { get; set; }
        //public string PasswordHash { get; set; }
        public string Address { get; set; }
        public string JobSource { get; set; }
        public string AgeBand { get; set; }
        public decimal ScoreCardValue { get; set; }
        public string TempRef { get; set; }
        public string LinkedIn { get; set; }
        public string Website { get; set; }
        public string Instagram { get; set; }
        public string Twitter { get; set; }
    }


    public class FetchJobApplicationBySourceNew
    {
        public string JobSource { get; set; }

        public long NoOfApplicant { get; set; }
        public int YearApplied { get; set; }

    }
    public class JobSourcesFromDb
    {
        public string JobSource { get; set; }
    }


    public class JobApplicantionSourceDto
    {
        public string JobSource { get; set; }

        public int NoOfApplicants { get; set; }

        public int YearApplied { get; set; }
    }

    public class ManageJobApplicationSkillDto
    {
        //public long ApplicantId { get; set; }
        public long ID { get; set; }
        public long? JobAppplicantId { get; set; }
        public string SkillName { get; set; }
        public int JobId { get; set; }
        public string FirstName { get; set; }
        public string ApplicantCode { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? LastUpdate { get; set; }
    }

    public class JobsByRoleDto
    {
        public string JobRoleName { get; set; }

        public int NoOfJobsPerRole { get; set; }

        public int YearCreated { get; set; }
    }

    public class InterviewHistoryDto
    {
        public Guid JobApplicationId { get; set; }

        public Guid JobApplicantId { get; set; }

        public string StatusDescription { get; set; }

        public DateTime DateTriggered { get; set; }
    }

    public class JobsByDepartmentDto
    {
        public string DepartmentName { get; set; }

        public int NoOfJobsPerDepartment { get; set; }

        public int YearCreated { get; set; }
    }

    public class JobRecruitmentOvertimeDto
    {
        public string MonthRecruited { get; set; }

        public int NoOfApplications { get; set; }

        //public int YearApplied { get; set; }
    }

    public class EmployeeSkillDistributionDto
    {
        public string SkillName { get; set; }

        public int NoOfEmployees { get; set; }

        //public int YearApplied { get; set; }
    }

    public class ApplicantDropOffStageDto
    {
        public string Stage { get; set; }

        public int NoOfDropOffStage { get; set; }

        //public int YearApplied { get; set; }
    }


    public class JobApplicationAgeBandDto
    {
        public string AgeBand { get; set; }

        public int NoOfApplicants { get; set; }

        public int YearApplied { get; set; }
    }

    public class JobApplicationMailDto
    {
        public Guid JobApplicantionId { get; set; }

        public Guid JobApplicantId { get; set; }

        public bool IsInprogress { get; set; }

        public Guid? HireStageId { get; set; }

        public Guid? EmailTemplateIdH { get; set; }

        public Guid? SubHireStageId { get; set; }

        public string ApplicationStatus { get; set; }

        public Guid? EmailTemplateIdS { get; set; }

        public Guid JobId { get; set; }

    }


    public class InvitedApplicantsListDto
    {
        public int NoOfInvitedApplicants { get; set; }

        public string StatusName { get; set; }
    }

    public class InterviewedApplicantsDto
    {
        public int NoOfInterviewedApplicants { get; set; }

        public string StatusName { get; set; }
    }

    public class ApplicantsPendingOfferListDto
    {
        public int NoOfPendingOffers { get; set; }

        public string StatusName { get; set; }
    }

    //-----------------------------------------------Track-------------------------------------------------//
    public class ApplicantJobSkillDto
    {
        public long ID { get; set; }
        public long ApplicantId { get; set; }
        public string SkillName { get; set; }
    }

    public class JobApplicationSearch : PaginationRequest
    {
        public Guid? JobApplicantId { get; set; }
        public string? JobTitle { get; set; }
        public DateTime? ApplicationDate { get; set; }
        public string? Status { get; set; }
    }



    public class JobApplicationListDto
    {
        public Guid JobId { get; set; }
        public Guid Id { get; set; }
        public string ApplicantCode { get; set; }
        public Guid JobApplicantId { get; set; }
        public DateTime DateApplied { get; set; }
        public DateTime? LastUpdate { get; set; }
        public bool AlertSent { get; set; }
        public string CoverLetter { get; set; }
        public int? JobLocation { get; set; }
        public bool IsHired { get; set; }
        public bool IsInProgress { get; set; }
        public string ApplicationStatus { get; set; }
        public Guid? CountryId { get; set; }
        public string Country { get; set; }
        public Guid? StateId { get; set; }
        public string State { get; set; }
        public DateTime? DateRecruited { get; set; }  //----//
        public string ApplicantName { get; set; }
        public string ApplicantEmail { get; set; }
        public string JobTitle { get; set; }
        public Guid CompanyId { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime DateCreated { get; set; }
        public Guid CreatedById { get; set; }
        public string? ModifiedById { get; set; }
        public DateTime? DateModified { get; set; }
        public Guid? HireStageId { get; set; }
        public Guid? SubHireStageId { get; set; }
        public string HireStageName { get; set; }
        public string SubHireStageName { get; set; }
        public string TestPassCode { get; set; }

        public decimal? ScoreCardValue { get; set; }
        public string? ScoreCardDetail { get; set; }
        public string? JobAppliactionCV { get; set; }
        public long? QuizScore { get; set; }

        public virtual JobDto Job { get; set; }
        public virtual JobApplicantDto JobApplicant { get; set; }
    }

    public class FetchApplicantDropOffStageDto
    {
        public int CountOfAppllicantDropOfStageId { get; set; }
        public string DropOffStageName { get; set; }
    }

    public class JobApplicationList : PaginationRequest
    {
        public Guid JobApplicantId { get; set; }
        public Guid JobId { get; set; }
        public DateTime ApplicationDate { get; set; }
    }

    public class JobApplicationUpcomingInterviews
    {
        public string ApplicantName { get; set; }
        public string JobRoleName { get; set; }
        public DateTime InterviewDate { get; set; }
        public long Duration { get; set; }
        //public int pageNumber { get; set; } = DefaultValueMaps.pageNumber;
        //public int pageSize { get; set; } = DefaultValueMaps.pageSize;
    }


    public class JobApplicationReportSearch : PaginationRequest
    {
        public EmploymentType? EmploymentType { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string SortBy { get; set; }
        public string SearchString { get; set; }
    }


    public class JobApplicationByRoleSearch : PaginationRequest
    {

        public long JobTitle { get; set; }
    }
    public class JobApplicationUpdateStatus
    {

        public string Status { get; set; }
        public Guid JobApplicantId { get; set; }

        public Guid HireStage { get; set; }

    }

    public class JobApplicantScheduleInterview
    {

        public Guid JobApplicantionId { get; set; }
        public List<Guid> InterviewerIds { get; set; }

        public DateTime InterviewDate { get; set; }
        public DateTime InterviewTime { get; set; }
        public long Duration { get; set; }
        public Guid InterviewType { get; set; }

        public string EmployeeName { get; set; }
        public Guid HireStageId { get; set; }
        public Guid SubHireStageId { get; set; }

    }
    public class ManageJobApplicantDTo : BaseResponseModel
    {
        [Required(ErrorMessage = "Enter First Name")]
        [StringLength(50, ErrorMessage = "First Name cannot be more than 50 characters")]
        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        [Required(ErrorMessage = "Enter Last Name")]
        [StringLength(50, ErrorMessage = "Last Name cannot be more than 50 characters")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Enter Email")]
        [RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", ErrorMessage = "Enter a valid email")]
        [StringLength(100, ErrorMessage = "Email cannot be more than 100 characters")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Enter Password")]
        [DataType(DataType.Password)]
        [StringLength(50, ErrorMessage = "Password cannot be more than 8 characters")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Passwords Don't Match ")]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ComfirmPassword { get; set; }

        public string PhoneNumber { get; set; }

        public string Code { get; set; }
    }

    public class MangeLoginJobApplicantDTO
    {
        public string UserName { get; set; }
        public string Password { get; set; }

        public string Email { get; set; }
    }

    public class MangeLoginJobApplicantWithSocialMediaDTO
    {
        public MangeLoginJobApplicantWithSocialMediaDTO()
        {
            DateCreated = DateTime.Now;
            IsActive = true;
            IsDeleted = false;

        }
        ///[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime DateCreated { get; set; }
        public long CreatedById { get; set; }
        public DateTime? DateModified { get; set; }
        public long? ModifiedById { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
    }


    public class RessetPasscode
    {
        public string Code { get; set; }


        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }


    public class ChangePassword
    {

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmNewPassword { get; set; }
    }

    public class JobApplicantListingDTO
    {
        public string JobRoleName { get; set; }
        public long JobTypeId { get; set; }
        public List<JobApplicantDto> JobApplicant { get; set; }

        public long TotalCount { get; set; }

    }




    public class JobApplicantSearchDTO : PaginationRequest
    {
        public string FullTime { get; set; }
        public string PartTime { get; set; }
        public string Name { get; set; }
        public string Remote { get; set; }
        public string Sector { get; set; }
        public string Contract { get; set; }
        public decimal MinAmount { get; set; }
        public decimal MaxAmount { get; set; }
    }


    public class ScheduleJobInterviewDto
    {
        //public long InterviewerEmployeeId { get; set; }
        public Guid JobApplicantId { get; set; }

        public DateTime InterviewDate { get; set; }
        public DateTime InterviewTime { get; set; }
        public long Duration { get; set; }
        public Guid InterviewType { get; set; }

        public List<InterviewerEmployeeIdDto> InterviewerEmployee { get; set; } = new List<InterviewerEmployeeIdDto>();

        public ScheduleJobInterviewDto()
        {
            InterviewerEmployee = new List<InterviewerEmployeeIdDto>();
        }
    }

    public class InterviewerEmployeeIdDto
    {
        public Guid InterviewerEmployeeId { get; set; }
    }

    public class AssingStatusToJobApplicationDto
    {
        public Guid ApplicantId { get; set; }
        public Guid ApplicationId { get; set; }
        public Guid HireStageId { get; set; }
        public Guid? HireSubStageId { get; set; }
    }

    public class RemoveShortlistedJobApplicationDto
    {
        public Guid ApplicationId { get; set; }
    }

    public class ApplicantQuizRecordDto
    {
        public Guid CompanyId { get; set; }
        public Guid Id { get; set; }
        public Guid QuizId { get; set; }
        public string? QuizName { get; set; }
        public Guid UserId { get; set; }
    }
    public class ApplyForJobDto
    {
        public Guid JobId { get; set; }
        public Guid ApplicantId { get; set; }
        public string? JobChannel { get; set; }
        public IFormFile ApplicantCV { get; set; }
    }

    public class CreateApplicantCVtoCVBank
    {
        public Guid ApplicantId { get; set; }
        public string ApplicantCVUrl { get; set; }
    }
    public class SaveApplicantScoreCardEvaluationDto
    {
        public Guid JobApplicantId { get; set; }
        public Guid JobApplicantionId { get; set; }
        public Guid HireStageId { get; set; }
        public Guid SubHireStageId { get; set; }
        public Guid InterviewerEmployeeId { get; set; }

    }
    public class SaveApplicantScoreCardDto
    {
        public Guid JobApplicantId { get; set; }
        public Guid InterviewerEmployeeId { get; set; }
        public Guid JobApplicantionId { get; set; }
        public Guid HireStageId { get; set; }
        public Guid SubHireStageId { get; set; }
        public List<QuestionsWithMarkDto> QuestionsWithMark { get; set; }
    }

    public class QuestionsWithMarkDto
    {
        public Guid QuestionId { get; set; }
        public Guid FocusAreaId { get; set; }
        public long Score { get; set; }
        public string Comment { get; set; }
    }

    public class ManageJobApplicationDto
    {
        public Guid Id { get; set; }
        public Guid JobApplicantId { get; set; }
        public Guid JobId { get; set; }
        public string FirstName { get; set; }
        public string ApplicantCode { get; set; }
        public DateTime DateApplied { get; set; }
        public DateTime? LastUpdate { get; set; }
        public bool AlertSent { get; set; }
        public string CoverLetter { get; set; }
        public Guid HireStageId { get; set; }
    }


    public class OnboardingData
    {
        public OnboardRecruitmentRequest JobApplicantOnboarding { get; set; }
    }

    public class JobApplicantOnboardingDetails
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string EmailAddress { get; set; }
        public string Address { get; set; }
        public DateTime? DateofBirth { get; set; }
        public string PhoneNumber { get; set; }
        public int? Gender { get; set; }
        public string MaritalStatus { get; set; }
        public string PersonalEmail { get; set; }
        public int ReligionId { get; set; }
        public string FieldOfStudy { get; set; }
        public string Degree { get; set; }
        public DateTime DateOfCompletion { get; set; }
        public string CGPA { get; set; }
        public string InstitusionId { get; set; }
        public string NextOfKinAddress { get; set; }
        public string NextOfKinName { get; set; }
        public string NextOfKinRelationship { get; set; }
        public string NextOfKinPhoneNumber { get; set; }
        public string State { get; set; }
        public string? Country { get; set; }
        public int OnboardingStatus { get; set; }
        public Guid CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        public Guid CompanyId { get; set; }
    }

    public class OnboardRecruitmentResponse
    {

        public string Id { get; set; }
        public string IsOfferSent { get; set; }
        public string IsSuccessful { get; set; }

    }

    public class OnboardRecruitmentRequest
    {
        public string CompanyId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string PhoneNumber { get; set; }
        public string DateOfBirth { get; set; }
        public string EmailAddress { get; set; }
        public string Gender { get; set; }
        public string MaritalStatus { get; set; }
        public string PersonalEmail { get; set; }
        public string ReligionId { get; set; }
        public string FieldOfStudy { get; set; }
        public string Degree { get; set; }
        public string DateOfCompletion { get; set; }
        public string CGPA { get; set; }
        public string InstitutionId { get; set; }
        public string NextOfKinName { get; set; }
        public string NextOfKinRelationship { get; set; }
        public string NextOfKinPhoneNumber { get; set; }
        public string NextOfKinAddress { get; set; }
        public string Address { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string JobTitleId { get; set; }
        public string JobTitleName { get; set; }
        public DateTime HiredDate { get; set; }
        public string SalaryPerAnnum { get; set; }
        public string Designation { get; set; }
        public string EmployeeType { get; set; }
        public string DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public string ReportingManager { get; set; }
        public string ReportingManagerId { get; set; }
        public string CompanyLocationId { get; set; }
        public string CompanyLocationName { get; set; }
        public string OfferLetterExpiration { get; set; }
    }




    public class OnboardingWorkInfo
    {
        public Guid JobTitleId { get; set; }
        public string JobTitleName { get; set; }
        public DateTime HiredDate { get; set; }
        public string SalaryPerAnum { get; set; }
        public string Designation { get; set; }
        public string EmployeeType { get; set; }
        public Guid DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public string ReportingManager { get; set; }
        public Guid ReportingManagerId { get; set; }
        public int LocationId { get; set; }
        public string LocationName { get; set; }
        public string OfferLetterExpiration { get; set; }
        public Guid OnboardInfoId { get; set; }
        public Guid CreatedBy { get; set; }
        public string CreatedByName { get; set; }

    }

    public class JobApplicantObj
    {
        public long ID { get; set; }
        public long? UserId { get; set; }
        public int TitleId { get; set; }
        public string FirstName { get; set; }
        public int company_id { get; set; }
        public string LastName { get; set; }
        public string OtherName { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public DateTime? BirthDate { get; set; }
        public long? StateOfResidentId { get; set; }
        public long? NationalityId { get; set; }
        public string Skills { get; set; }
        public int? YearOfExperience { get; set; }
        public DateTime? LastUpdate { get; set; }
        public string Password { get; set; }
        public int? Gender { get; set; }
        public string Title { get; set; }
        public int? NYSCStatus { get; set; }
        public string WorkAchievements { get; set; }
        public string CVName { get; set; }
        public string CVMimeType { get; set; }
        public string DesiredSalaryCurrency { get; set; }
        public decimal? DesiredSalaryAnnum { get; set; }
        public bool DesiredSalaryNegotiable { get; set; }
        public bool WillingToRelocate { get; set; }
        public string PhotoName { get; set; }
        public string PhotoMimeType { get; set; }
        public bool? CompleteProfile { get; set; }
        public int Age { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public string Address { get; set; }
        public string Education { get; set; }
        public string WorkExperience { get; set; }
        public string Reference { get; set; }
        public string TempRef { get; set; }
        public string LinkedIn { get; set; }
        public string Website { get; set; }
        public string Instagram { get; set; }
        public string Twitter { get; set; }
        public int? SkillId { get; set; }
        public string Skill { get; set; }
        public string session_token { get; set; }
        public string jwt_token { get; set; }
        public string user_token { get; set; }
        public List<string> lstPermissions { get; set; }
    }



    public class JobApplicantWorkExperienceDTO
    {
        public long ID { get; set; }
        public string JobTitle { get; set; }
        public string CompanyName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Role { get; set; }
        public long JobApplicantId { get; set; }
        public string Description { get; set; }
        public bool WorkHere { get; set; }

    }

    public class JobApplicantEducationDTO
    {
        public long ID { get; set; }
        public string Certification { get; set; }
        public string School { get; set; }
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; } //--///
        public string Degree { get; set; }
        public string Course { get; set; }
        public string Grade { get; set; }
        public string Country { get; set; }

        public string Others { get; set; }
        public string OthersSchools { get; set; }
        public string OtherCourses { get; set; }

    }

    public class ApplicantDocDTO
    {
        public string DocumentType { get; set; }
        public string DocumentTitle { get; set; }
        public string FilePath { get; set; }
        public long JobApplicantEducationID { get; set; }
        public long JobApplicantReferenceID { get; set; }
        public IFormFile Files { get; set; }
    }

    public class ApplicantPhotoDTO
    {
        //public string PhotoMimeType { get; set; }
        public string PhotoName { get; set; }
        public string FilePath { get; set; }
        public long JobApplicantID { get; set; }
        public IFormFile Files { get; set; }
    }


    public class JobApplicantReferencesDTO
    {
        public long ID { get; set; }
        public string FullName { get; set; }
        public string Profession { get; set; }
        public string PlaceOfWork { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public long JobApplicantId { get; set; }
    }
    public class InterviwerListforSharingDTO
    {
        public Guid InterviewerEmployeeId { get; set; }
        //public EmployeeDTO EmployeeDTO { get; set; }
        public Guid JobApplicantId { get; set; }

        public DateTime InterviewDate { get; set; }
        public DateTime InterviewTime { get; set; }
        public long Duration { get; set; }
        public Guid InterviewType { get; set; }
    }

    public class ApplicantInterviewScheduleDTO
    {
        public Guid JobApplicantId { get; set; }
        public string JobApplicantName { get; set; }
        public Guid InterviewerEmployeeId { get; set; }
        public string JobRoleName { get; set; }
        public DateTime InterviewDate { get; set; }
        public DateTime InterviewTime { get; set; }
        public long Duration { get; set; }

        public Guid JobApplicantionId { get; set; }
        public bool IsShared { get; set; }
        public Guid? HireStageId { get; set; }
        public Guid? SubhireStageId { get; set; }

        public Guid InterviewType { get; set; }

    }

    public class JobApplicationListingDTO : PaginationRequest
    {
        public Guid DepartmentId { get; set; }
        public string JobTitle { get; set; }
        public long JobApplicants { get; set; }
        public long TotalCount { get; set; }
    }
    public class ApplicantInterviewScheduleFilterDTO : PaginationRequest
    {
        public string? JobApplicantName { get; set; }
        public Guid? JobApplicantId { get; set; }
        public Guid? JobTitle { get; set; }
        public DateTime? InterviewDate { get; set; }
        public long? Duration { get; set; }
        public bool? CompletedInterview { get; set; }
    }

    public class JobApplicationSearchDTO
    {
        public string JobTitle { get; set; }
        public DateTime DatePosted { get; set; }
        public long Applicants { get; set; }
        public string status { get; set; }
        public JobApplicationDto JobApplications { get; set; }
    }
    public class EvaluationRecruitmentFocusAreaDTO
    {
        public Guid Id { get; set; }
        public Guid RecruitmentScoreCardId { get; set; }
        public string FocusArea { get; set; }
        public decimal TotalWeight { get; set; }
        public Guid? ScoringTypeId { get; set; }

        public List<EvaluationScoreCardQuestionDTO> ScoreCardQuestions { get; set; }
    }
    public class EvaluationRecruitmentScoreCardDTO
    {
        public Guid Id { get; set; }
        public string name { get; set; }
        public Guid CompanyId { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime DateCreated { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime? DateModified { get; set; }
        public string? ModifiedById { get; set; }
        public List<EvaluationRecruitmentFocusAreaDTO> FocusAreas { get; set; }
        public List<ScoreCardQuestionDto> Questions { get; set; }
    }
    public class RecruitmentScoreCardDTO
    {
        public Guid Id { get; set; }
        public string FocusArea { get; set; }
        public decimal TotalWeight { get; set; }
        public Guid? ScoringTypeId { get; set; }
        public Guid CompanyId { get; set; }
        public int SubID { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime DateCreated { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime? DateModified { get; set; }
        public Guid? ModifiedById { get; set; }
        public List<RecruitmentFocusAreaDto> FocusAreas { get; set; }
        public List<ScoreCardQuestionDto> Questions { get; set; }
    }
    public class RecruitmentScoreCardFocusAreaDTO
    {
        public Guid Id { get; set; }
        public string FocusArea { get; set; }
        public decimal TotalWeight { get; set; }
        public Guid? ScoringTypeId { get; set; }
        public Guid CompanyId { get; set; }
        public int SubID { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime DateCreated { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime? DateModified { get; set; }
        public Guid? ModifiedById { get; set; }
        public List<ScoreCardQuestionDto> Questions { get; set; }
    }



    public class JobApplicantDetailDto
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }
        public string Social { get; set; }
        public string Twitter { get; set; }
        public string Linkedin { get; set; }
        public string Instagram { get; set; }
        public string Address { get; set; }
        public string AgeRange { get; set; }
        public string PhoneNumber { get; set; }
        public string CountryCode { get; set; }

        public string? ProfileImage { get; set; }
        public Guid UserId { get; set; }

        public DateTime DateOfBirth { get; set; }
    }

    public class ManageRecruitmentScoreCardDTO
    {
        public Guid Id { get; set; }
        public string FocusArea { get; set; }
        public decimal TotalWeight { get; set; }
        public Guid? ScoringTypeId { get; set; }

        public string Description { get; set; }
    }

    public class ManageRecruitmentScoreCardFocusAreaDTO
    {
        public Guid Id { get; set; }
        public Guid ScoredCardId { get; set; }
        public string FocusArea { get; set; }
        public decimal TotalWeight { get; set; }
        public Guid? ScoringTypeId { get; set; }

        public string Questions { get; set; }
    }
    public class EvaluationScoreCardQuestionDTO
    {
        public Guid Id { get; set; }
        public string Question { get; set; }
        public string Comment { get; set; }
        public decimal Weight { get; set; }
        public Guid RecruitmentFocusAreaId { get; set; }
        public long Score { get; set; }


    }


    public class ManageRecruitmentApplicantScoreCardDTO
    {
        public long ID { get; set; }
        public string FocusArea { get; set; }
        public decimal TotalWeight { get; set; }
        public int? ScoringTypeId { get; set; }

        public string comments { get; set; }

        public string ApplicantName { get; set; }

        public string PositionTitle { get; set; }


    }

    public class JobApplicationDto
    {
        public Guid Id { get; set; }
        public Guid JobId { get; set; }
        public string ApplicantCode { get; set; }
        public Guid JobApplicantId { get; set; }
        public DateTime DateApplied { get; set; }
        public DateTime? LastUpdate { get; set; }
        public bool AlertSent { get; set; }
        public string CoverLetter { get; set; }
        public int? JobLocation { get; set; }
        public bool IsHired { get; set; }
        public string ApplicationStatus { get; set; }
        public DateTime? DateRecruited { get; set; }
        public Guid? HireStageId { get; set; }

        public string TestPassCode { get; set; }

        public decimal? ScoreCardValue { get; set; }
        public string? ScoreCardDetail { get; set; }
        public Guid? SubhireStageId { get; set; }
        public bool IsInProgress { get; set; }
        public bool IsDeleted { get; set; }
        public Guid CompanyId { get; set; }
        public string Fullname { get; set; }
        public string HireStageName { get; set; }
        public virtual JobDto Job { get; set; }
        public DateTime? DateCreated { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime? DateModified { get; set; }
        public string? ModifiedById { get; set; }
        public virtual ApplicationProfileDto JobApplicant { get; set; }
        public virtual ICollection<JobInterviewHistoryDto> InterviewHistory { get; set; }
    }

    public class JobInterviewHistoryDto
    {
    }

    public class ApplicationProfileDto
    {
    }

    public class JobApplicantByJobTitleFilter : PaginationRequest
    {
        public string JobTitle { get; set; }
    }

    public class FetchJobApplicantByJobTitle
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public string Fullname { get; set; }
        public string JobTitle { get; set; }
        public string Email { get; set; }
        public string Status { get; set; }

    }
    public class JobListDto
    {
        public Guid JobId { get; set; }
        public string Department { get; set; }
        public string JobTitle { get; set; }
        public long NumberOfApplicant { get; set; }
        public DateTime? DatePosted { get; set; }
        public string Status { get; set; }
        public DateTime? ScheduleDate { get; set; }
        public string? SalaryRange { get; set; }
        public EmploymentType EmploymentType { get; set; }
        public string? EmploymentTypeText { get; set; }
        public Guid CompanyId { get; set; }
        public JobPostStatus JobPostStatus { get; set; }
        public JobStatus JobStatus { get; set; }
        public string? JobStatusText { get; set; }
        public DateTime? DateCreated { get; set; }
        public string JobDescription { get; set; }
        public Guid StateId { get; set; }
        public Guid CountryId { get; set; }
        public string StateName { get; set; }
        public string? CountryName { get; set; }
        public string? DepartmentName { get; set; }
        public string Experience { get; set; }
        public decimal? SalaryRangeFrom { get; set; }
        public decimal? SalaryRangeTo { get; set; }

    }

    public class JobListFilterDto : PaginationRequest
    {
        public JobPostStatus? JobPostStatus { get; set; }
        public EmploymentType? EmploymentType { get; set; }
        public decimal? SalaryRangeFrom { get; set; }
        public decimal? SalaryRangeTo { get; set; }
        public string? JobTitle { get; set; }
        public Guid? DepartmentId { get; set; }
        public DateTime? DatePosted { get; set; }
        public JobStatus? Status { get; set; }
        public string? search { get; set; }
        public string? JobExperienceLevel { get; set; }

    }


    public class DepartmentDataFromGRPCList
    {
        public List<DepartmentDataFromGRPC> departmentDataFromGRPCs { get; set; }
    }

    public class DepartmentDataFromGRPC
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string HeadOfDepartment { get; set; }
        public string HODName { get; set; }
    }

    public class SupportOfficerApplication
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string JobTitle { get; set; }
        public decimal? EvaluationScore { get; set; }
        public long? QuizScore { get; set; }
        public string Status { get; set; }
        public DateTime DateApplied { get; set; }
    }

    public class SupportOfficerApplicationFilter : PaginationRequest
    {
        public string? JobTitle { get; set; }
    }

    public class ApplicantSelection
    {
        public Guid ApplicantId { get; set; }
        public Guid ApplicationId { get; set; }
        public string FullName { get; set; }
        public DateTime DateApplied { get; set; }
        public string JobTitle { get; set; }
        public string JobPostText { get; set; }
        public JobPostStatus JobPost { get; set; }
    }

    public class ApplicantSelectionFilter : PaginationRequest
    {
        public JobPostStatus? JobPost { get; set; }
        public string? Search { get; set; }
        public string? JobTitle { get; set; }

    }

    public class CandidateApplicantSelection
    {
        public Guid ApplicantId { get; set; }
        public Guid ApplicationId { get; set; }
        public Guid JobId { get; set; }
        public string FullName { get; set; }
        public DateTime DateApplied { get; set; }
        public string JobTitle { get; set; }
        public string JobPostText { get; set; }
        public JobPostStatus JobPost { get; set; }
        public string Email { get; set; }
        public decimal? EvaluationScore { get; set; }
        public long? QuizScore { get; set; }
        public string Status { get; set; }
        public string? StageName { get; set; }
    }

    public class CandidateApplicantSelectionFilter : PaginationRequest
    {
        public string? Search { get; set; }
        public string? JobTitle { get; set; }
        public int? RangeScoreCardStart { get; set; }
        public int? RangeScoreCardEnd { get; set; }
        public int? RangeQuizStart { get; set; }
        public int? RangeQuizEndEnd { get; set; }
    }

    public class EmployeeDetails
    {
        public string FullName { get; set; }
        public string Phonenumber { get; set; }
        public string Email { get; set; }
        public string ProfileImage { get; set; }
        public string EmployeeId { get; set; }
    }


    public class CalenderInterviewDay
    {
        public int DayOfMonth { get; set; }
        public bool HasInterview { get; set; }
    }

    public class ReveiwerUpComingTaskDto
    {
        public string TaskDescription { get; set; }
        public DateTime DateCreated { get; set; }
        public string FormatedCreated { get; set; }
        public DateTime UpComingTaskDate { get; set; }
    }

    public class SaveJobApplicantScoreCardDto
    {
        public Guid JobApplicantionId { get; set; }
        public decimal ScoreCardScore { get; set; }
    }
}