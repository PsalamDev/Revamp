namespace Domain.Entities
{
    public class ApplicantAnswer : BaseEntity
    {
        public long JobApplicantId { get; set; }
        public long JobQuestionId { get; set; }
        public string Answer { get; set; }
    }
}