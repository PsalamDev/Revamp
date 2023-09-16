using HRShared.Common;

namespace Core.Common.Model
{
    public class QuizModel
    {

    }

    public class ManageQuizDTO
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public int TypeId { get; set; }

        public string Questions { get; set; }
        public int? Duration { get; set; }
    }

    public class QuizDTO
    {
        public QuizDTO()
        {
            Questions = new List<QuestionDTO>();
        }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int TypeId { get; set; }
        public Guid CompanyID { get; set; }

        public List<QuestionDTO> Questions { get; set; }
        public int TotalQuestions => Questions.Count;
        public bool IsDeleted { get; set; }
        public DateTime DateCreated { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime? DateModified { get; set; }
        public Guid? ModifiedById { get; set; }
        public int? Duration { get; set; }
    }

    public class QuestionDTO
    {
        public QuestionDTO()
        {
            QuestionOptions = new List<QuestionOptionDTO>();
        }
        public Guid Id { get; set; }
        public string QuestionText { get; set; }
        public int TypeId { get; set; }
        public string Type { get; set; }
        public Guid QuizId { get; set; }
        public string QuizName { get; set; }

        public List<QuestionOptionDTO> QuestionOptions { get; set; }
        public int TotalAnswers => QuestionOptions.Count;
    }

    public class QuestionOptionDTO
    {
        public Guid Id { get; set; }
        public Guid QuestionId { get; set; }
        public string Question { get; set; }
        public string Value { get; set; }
        public bool IsAnswer { get; set; }
    }


    public class QuizAnswerDTO
    {
        public Guid QuizId { get; set; }
        public List<Answer> Answers { get; set; }
    }

    public class Answer
    {
        public Guid QuestionId { get; set; }
        public Guid AnswerId { get; set; }
    }

    public class IDTextViewModel
    {
        public int Id { get; set; }
        public string Text { get; set; }
    }


    public class Quizfilter : PaginationRequest
    {
        public int? StartRange { get; set; }
        public int? EndRange { get; set; }
    }

    public class ApplicantQuizfilter : PaginationRequest
    {
        public int? StartRange { get; set; }
        public int? EndRange { get; set; }
        public Guid ApplicantId { get; set; }
    }
}