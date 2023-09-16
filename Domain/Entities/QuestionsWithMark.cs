namespace Domain.Entities
{
    public class QuestionsWithMark : BaseEntity
    {
        public Guid AggregateScoresId { get; set; }
        public Guid ScoreCardId { get; set; }
        public Guid QuestionId { get; set; }
        public Guid FocusAreaId { get; set; }
        public long Score { get; set; }
        public string Comment { get; set; }
        public Guid CompanyId { get; set; }
        public AggregateScore AggregateScores { get; set; }
    }
}