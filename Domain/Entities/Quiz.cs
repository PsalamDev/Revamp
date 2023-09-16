namespace Domain.Entities
{
    public class Quiz : BaseEntity
    {
        public Guid CompanyId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int PsychometricTypeId { get; set; }
        public int? Duration { get; set; }
        public virtual PsychometricType PsychometricType { get; set; }
        public virtual ICollection<Question> Questions { get; set; }
    }
}