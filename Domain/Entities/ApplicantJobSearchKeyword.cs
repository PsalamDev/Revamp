

namespace Domain.Entities
{
    public class ApplicantJobSearchKeyword : BaseEntity
    {
        public Guid ApplicantProfileId { get; set; }
        public Guid CompanyId { get; set; }
        public string Jobkeyword { get; set; }
        public ApplicantProfile ApplicantProfile { get; set; }

    }
}
