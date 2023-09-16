namespace Domain.Entities
{
    public class QuestionOption
    {
        public Guid Id { get; set; }
        public Guid QuestionId { get; set; }
        public string Question { get; set; }
        public string Value { get; set; }
        public bool IsAnswer { get; set; }
        public int? Score { get; set; }

        public bool IsDeleted { get; set; }

        public string? ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public Guid? CreatedBy { get; set; }
    }
}