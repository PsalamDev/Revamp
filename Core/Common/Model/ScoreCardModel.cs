using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Common.Model
{
    public class ScoreCardModel : BaseResponseModel
    {
        public string ScoreCardName { get; set; }
        public string Description { get; set; }
        public Guid CompanyId { get; set; }
        public ICollection<RecruitmentFocusAreaModel>? RecruitmentFocusAreaModels { get; set; }
    }

    public class CreateScoreCardRequestModel
    {
        public string ScoreCardName { get; set; }
        public string Description { get; set; }
    }

    public class UpdateScoreCardRequestModel
    {
        public Guid Id { get; set; }
        public string ScoreCardName { get; set; }
        public string Description { get; set; }
    }

    public class ScoreCardDto
    {
        public Guid Id { get; set; }
        public string? ScoreCardName { get; set; }
        public string? Description { get; set; }
        public Guid CompanyId { get; set; }
        public List<RecruitmentFocusAreaDto>? RecruitmentFocusAreas { get; set; }
        public List<ScoreCardQuestionDto>? ScoreCardQuestions { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime DateCreated { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime? DateModified { get; set; }
        public Guid? ModifiedById { get; set; }
        public int? Duration { get; set; }
    }
}
