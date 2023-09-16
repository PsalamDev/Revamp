using static Domain.Enums.Enum;

namespace Domain.Entities
{
    public class JobScheduleInterview : BaseEntity
    {
        public Guid InterviewerEmployeeId { get; set; }
        public DateTime InterviewDate { get; set; }
        public DateTime InterviewTime { get; set; }
        public long Duration { get; set; }
        public Guid JobApplicantionId { get; set; }
        public bool IsShared { get; set; }
        public Guid? HireStageId { get; set; }
        public Guid? SubhireStageId { get; set; }
        public Guid InterviewType { get; set; }
        public InterViewStatus? Status { get; set; }
        public Guid CompanyId { get; set; }
        public bool IsActive { get; set; }
        public string JobTitle { get; set; }
        public JobApplication JobApplication { get; set; }
        public Stage? HireStage { get; set; }
        public SubStage? SubhireStage { get; set; }

    }
}
