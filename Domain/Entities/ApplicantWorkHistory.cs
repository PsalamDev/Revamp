namespace Domain.Entities
{
    public class ApplicantWorkHistory : BaseEntity
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
}