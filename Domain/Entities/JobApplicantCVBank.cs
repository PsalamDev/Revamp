namespace Domain.Entities
{
    public class JobApplicantCVBank : BaseEntity
    {
        public Guid? ApplicantionId { get; set; }
        public Guid? ApplicantId { get; set; }
        public string? FileType { get; set; }
        public string? FileName { get; set; }
        public string CVFileUrl { get; set; }
        public Guid CompanyId { get; set; }
        public JobApplication? JobApplication { get; set; }
        public ApplicantProfile? ApplicantProfile { get; set; }

    }
}