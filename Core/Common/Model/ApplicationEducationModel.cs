using HRShared.Common;

namespace Core.Common.Model
{
    public class ApplicationEducationModel
    {

    }

    public class ApplicantQualificationResponse : BaseResponseModel
    {
        public Guid Id { get; set; }
        public Guid ApplicantId { get; set; }
        public Guid QualificationType { get; set; }
        public string ProgramTypeName { get; set; }
        public string InstitutionName { get; set; }

        public string CGPA { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsOngoing { get; set; }
        public string? Country { get; set; }
    }

    public class ApplicantQualificationListResponse : BaseResponseModel
    {
        public Guid ApplicantId { get; set; }
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
        public string? Country { get; set; }
    }


    public class ApplicantQualificationRequest
    {

        public Guid ApplicantId { get; set; }
        public Guid QualificationType { get; set; }
        public string QualificationDegreeName { get; set; }
        public string ProgramTypeName { get; set; }
        public string InstitutionName { get; set; }
        public string? CGPA { get; set; }
        public Guid? CourseId { get; set; }
        public string CourseName { get; set; }
        public string? GradeId { get; set; }
        public string? GradeName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsOngoing { get; set; }
        public string? Country { get; set; }
    }

    public class UpdateApplicantQualificationRequest
    {
        public Guid Id { get; set; }
        public Guid ApplicantId { get; set; }

        public Guid QualificationType { get; set; }
        public string QualificationDegreeName { get; set; }
        public string ProgramTypeName { get; set; }
        public string InstitutionName { get; set; }
        public string? CGPA { get; set; }
        public Guid? CourseId { get; set; }
        public string CourseName { get; set; }
        public string? GradeId { get; set; }
        public string? GradeName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsOngoing { get; set; }
        public string? Country { get; set; }
    }

    public class QualificationListRequest : PaginationRequest
    {

        public Guid ApplicantId { get; set; }


    }
}