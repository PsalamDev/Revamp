using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ScoreCardQuestion : BaseEntity
    {
        public string Question { get; set; }
        public decimal Weight { get; set; }
        public Guid RecruitmentFocusAreaId { get; set; }
        public RecruitmentFocusArea FocusArea { get; set; }
        public Guid CompanyId { get; set; }

    }
}
