using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class AggregateScore : BaseEntity
    {
        public Guid JobApplicantId { get; set; }
        public Guid InterviewerEmployeeId { get; set; }
        public Guid JobApplicantionId { get; set; }
        public Guid HireStageId { get; set; }
        public Guid SubHireStageId { get; set; }
        public Guid ScoreCardId { get; set; }
        public double WeightedScore { get; set; }
        public Guid CompanyId { get; set; }
        public virtual ICollection<QuestionsWithMark> QuestionsWithMarks { get; set; }
    }
}
