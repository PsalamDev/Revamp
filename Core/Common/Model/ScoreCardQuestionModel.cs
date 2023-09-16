using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Common.Model
{
    public class ScoreCardQuestionModel : BaseResponseModel
    {
        public string Question { get; set; }
        public decimal Weight { get; set; }
        public Guid RecruitmentFocusAreaId { get; set; }
        public Guid CompanyId { get; set; }
    }

    public class CreateScoreCardQuestionModel
    {
        public string Question { get; set; }
        public decimal Weight { get; set; }
    }

    public class UpdateScoreCardQuestionModel
    {
        public Guid Id { get; set; }
        public string Question { get; set; }
        public decimal Weight { get; set; }
    }


    public class ScoreCardQuestionDto
    {
        public Guid Id { get; set; }
        public string Question { get; set; }
        public Guid? QuestionId { get; set; }
        public decimal Weight { get; set; }
        public Guid RecruitmentFocusAreaId { get; set; }
        public Guid CompanyId { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime DateCreated { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime? DateModified { get; set; }
        public Guid? ModifiedById { get; set; }
        public int? Duration { get; set; }
        public long? Score { get; set; }
        public string? Comment { get; set; }
    }
}
