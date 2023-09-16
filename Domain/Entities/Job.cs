using static Domain.Enums.Enum;

namespace Domain.Entities
{
    public class Job : BaseEntity
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
        public string? SalaryRange { get; set; }

        public Guid? ScoreCardId { get; set; }
        public Guid? QuizId { get; set; }
        public DateTime? DatePosted { get; set; }
        public DateTime? PostValidityFrom { get; set; }
        public DateTime? PostValidityTo { get; set; }
        public bool AlertSent { get; set; }
        public DateTime? SchedulePostDate { get; set; }
        public Guid CompanyId { get; set; }
        public JobPostStatus JobPostStatus { get; set; }
        public JobStatus JobStatus { get; set; } = JobStatus.Open;

        public decimal? SalaryRangeFrom { get; set; }
        public decimal? SalaryRangeTo { get; set; }

        public ICollection<JobReviewer>? JobReviewers { get; set; } = new HashSet<JobReviewer>();
        public ICollection<JobApplication> JobApplicants { get; set; } = new HashSet<JobApplication>();
    }
}
