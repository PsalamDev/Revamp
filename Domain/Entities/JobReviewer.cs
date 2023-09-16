using static Domain.Enums.Enum;

namespace Domain.Entities
{
    public class JobReviewer : BaseEntity
    {
        public Guid JobId { get; set; }
        public string ReviewerName { get; set; }
        public Guid ReviewerId { get; set; }
        public string ReviewerEmail { get; set; }
        public string? Comment { get; set; }
        public ReviwerStatus? Status { get; set; }
        public bool? IsReviewd { get; set; }
        public Job? Job { get; set; }
        public Guid? CompanyId { get; set; }

    }
}
