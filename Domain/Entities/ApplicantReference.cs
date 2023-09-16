namespace Domain.Entities
{
    public class ApplicantReference : BaseEntity
    {
        public Guid CompanyId { get; set; }
        public string FullName { get; set; }
        public string Profession { get; set; }
        public string PlaceOfWork { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public Guid JobApplicantId { get; set; }
        public string? Address { get; set; }
    }
}