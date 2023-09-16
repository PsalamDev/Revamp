using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Common.Model
{
    public class RecruitmentFocusAreaModel : BaseResponseModel
    {
        public Guid ScoreCardId { get; set; }
        public string FocusArea { get; set; }
        public decimal TotalWeight { get; set; }
        public Guid CompanyId { get; set; }
        public ICollection<ScoreCardQuestionModel> ScoreCardQuestions { get; set; }
    }

    public class CreateRecruitmentFocusAreaRequestModel
    {
        public Guid ScoreCardId { get; set; }
        public string FocusArea { get; set; }
        public decimal TotalWeight { get; set; }
        public List<CreateScoreCardQuestionModel> ScoreCardQuestions { get; set; }
    }



    public class UpdateRecruitmentFocusAreaRequestModel
    {
        public Guid Id { get; set; }
        public Guid ScoreCardId { get; set; }
        public string FocusArea { get; set; }
        public decimal TotalWeight { get; set; }
        public List<UpdateScoreCardQuestionModel> ScoreCardQuestions { get; set; }
    }

    public class RecruitmentFocusAreaDto
    {
        public Guid Id { get; set; }
        public Guid ScoreCardId { get; set; }
        public string FocusArea { get; set; }
        public decimal TotalWeight { get; set; }
        public Guid CompanyId { get; set; }
        public List<ScoreCardQuestionDto>? ScoreCardQuestions { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime DateCreated { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime? DateModified { get; set; }
        public Guid? ModifiedById { get; set; }
        public int? Duration { get; set; }
    }


}
