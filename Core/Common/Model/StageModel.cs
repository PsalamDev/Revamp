using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Common.Model
{
    public class StageModel : BaseResponseModel
    {
        public string StageName { get; set; }
        public bool EmailAutoResponde { get; set; }
        public Guid? EmailTemplateId { get; set; }
        public Guid? ScoreCardId { get; set; }
        public string? ScoreCardName { get; set; }
        public string? Description { get; set; }
        public Guid CompanyId { get; set; }
        public ICollection<SubStageModel>? SubStageModels { get; set; }
    }

    public class StagesModel : BaseResponseModel
    {
        public string StageName { get; set; }
        public bool EmailAutoResponde { get; set; }
        public Guid? EmailTemplateId { get; set; }
        public Guid? ScoreCardId { get; set; }
        public Guid? CompanyId { get; set; }
        public string? ScoreCardName { get; set; }
        public string? Description { get; set; }
        public ICollection<SubStagesModel>? SubStageModels { get; set; }
    }

    public class StagesRequestModel
    {
        public string StageName { get; set; }
        public bool EmailAutoResponde { get; set; }
        public Guid? EmailTemplateId { get; set; }
        public Guid CompanyId { get; set; }
        public Guid? ScoreCardId { get; set; }
        public string? ScoreCardName { get; set; }
        public string? Description { get; set; }
    }

    public class CreateStageRequestModel
    {
        public string StageName { get; set; }
        public bool EmailAutoResponde { get; set; }
        public Guid? EmailTemplateId { get; set; }
        public Guid? ScoreCardId { get; set; }
    }

    public class UpdateStageRequestModel
    {
        public Guid Id { get; set; }
        public string StageName { get; set; }
        public bool EmailAutoResponde { get; set; }
        public Guid? EmailTemplateId { get; set; }
        public Guid? ScoreCardId { get; set; }
    }

    public class StagePaginatedRequest : BaseQueryModel
    {

    }

    public class GetStages : BaseQueryModel
    {
        public Guid StageId { get; set; }
    }


}
