using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Common.Model
{
    public class SubStageModel : BaseResponseModel
    {
        public string SubStageName { get; set; }
        public bool EmailAutoResponde { get; set; }
        public Guid? EmailTemplateId { get; set; }
        public Guid CompanyId { get; set; }
        public Guid? StageId { get; set; }
        public string StageName { get; set; }
        public Guid? ScoreCardId { get; set; }
        public string? ScoreCardName { get; set; }
        public string? Description { get; set; }
    }

    public class SubStagesModel : BaseResponseModel
    {
        public string SubStageName { get; set; }
        public bool EmailAutoResponde { get; set; }
        public Guid? EmailTemplateId { get; set; }
        public Guid CompanyId { get; set; }
        public Guid? StageId { get; set; }
        public string? StageName { get; set; }
        public Guid? ScoreCardId { get; set; }
        public string? ScoreCardName { get; set; }
        public string? Description { get; set; }
    }

    public class SubStagesRequestModel
    {
        public string SubStageName { get; set; }
        public bool EmailAutoResponde { get; set; }
        public Guid? EmailTemplateId { get; set; }
        public Guid StageId { get; set; }
        public Guid? ScoreCardId { get; set; }
        public string? ScoreCardName { get; set; }
        public string? Description { get; set; }
    }

    public class CreateSubStageRequestModel
    {
        public string SubStageName { get; set; }
        public bool EmailAutoResponde { get; set; }
        public Guid? EmailTemplateId { get; set; }
        public Guid HirinStageId { get; set; }
        public string? ScoreCardId { get; set; }
    }

    public class UpdateSubStageRequestModel
    {
        public Guid Id { get; set; }
        public string SubStageName { get; set; }
        public bool EmailAutoResponde { get; set; }
        public Guid? EmailTemplateId { get; set; }
        public Guid StageId { get; set; }
        public string? ScoreCardId { get; set; }
    }


    public class Reviewer
    {
        public Guid ReviewerId { get; set; }
        public string ReviewerName { get; set; }
        public string ReviewerEmail { get; set; }
    }

    public class SubStagePaginatedRequest : BaseQueryModel
    {

    }

    public class GetUpdateSubStages : BaseQueryModel
    {
        public Guid SubStageId { get; set; }
    }
}
