using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class JobApplicationStageHistory : BaseEntity
    {
        public string StatusDescription { get; set; }
        public Guid? SubStageId { get; set; }
        public Guid JobApplicationId { get; set; }
        public Guid StageId { get; set; }
        public Guid CompanyId { get; set; }
        public JobApplication JobApplication { get; set; }
        public Stage Stage { get; set; }
    }
}
