using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Common.Model
{
    public class JobApplicationStageHistoryModel
    {
        public string StatusDescription { get; set; }
        public Guid? SubStageId { get; set; }
        public Guid JobApplicationId { get; set; }
        public Guid StageId { get; set; }
        public Guid CompanyId { get; set; }
        public DateTime RecruitmentActionDate { get; set; }
    }
}
