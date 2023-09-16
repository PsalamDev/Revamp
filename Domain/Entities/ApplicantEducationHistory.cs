namespace Domain.Entities
{
    public class ApplicantEducationHistory : BaseEntity
    {
        public Guid ApplicantId { get; set; }
        public Guid QualificationType { get; set; }
        public string QualificationDegreeName { get; set; }
        public string ProgramTypeName { get; set; }
        public string InstitutionName { get; set; }
        public string? CGPA { get; set; }
        public Guid? CourseId { get; set; }
        public string? CourseName { get; set; }
        public Guid? GradeId { get; set; }
        public string? GradeName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsOngoing { get; set; }
        public string? Country { get; set; }
    }
}