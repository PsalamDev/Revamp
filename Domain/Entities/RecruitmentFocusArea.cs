using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class RecruitmentFocusArea : BaseEntity
    {
        public RecruitmentFocusArea()
        {
            ScoreCardQuestions = new HashSet<ScoreCardQuestion>();
        }
        public Guid ScoreCardId { get; set; }
        public string FocusArea { get; set; }
        public decimal TotalWeight { get; set; }
        public ScoreCard ScoreCard { get; set; }
        public Guid CompanyId { get; set; }
        public ICollection<ScoreCardQuestion> ScoreCardQuestions { get; set; }
    }
}
