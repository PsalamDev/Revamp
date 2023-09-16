namespace Domain.Entities
{
    public class ApplicantQuizRecord : BaseEntity
    {
        public Guid CompanyId { get; set; }
        public Guid? ApplicantProfileId { get; set; }
        public Guid? JobApplicationId { get; set; }
        public Guid QuizId { get; set; }
        public int? Totalscore { get; set; }
        public int? TotalCorrectAnswer { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int Duration { get; set; }
        public bool? Iscompleted { get; set; }

        public ApplicantProfile? ApplicantProfile { get; set; }
        public JobApplication? JobApplication { get; set; }
        public Quiz? Quiz { get; set; }
    }
}