using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Stage : BaseEntity
    {
        public string StageName { get; set; }
        public bool EmailAutoResponde { get; set; }
        public Guid? EmailTemplateId { get; set; }
        public Guid CompanyId { get; set; }
        public Guid? ScoreCardId { get; set; }
        public ScoreCard? ScoreCard { get; set; }
        public ICollection<SubStage>? SubStages { get; set; }

    }
}
