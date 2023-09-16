namespace Domain.Entities
{
    public class ApplicantSkill : BaseEntity
    {
        public Guid CompanyId { get; set; }
        public Guid ApplicantsId { get; set; }
        public string SkillName { get; set; }
    }
}