using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Question : BaseEntity
    {
        public Question()
        {
            QuestionOptions = new HashSet<QuestionOption>();
        }
        public string QuestionText { get; set; }

        /// <summary>
        /// This represent the Type of Question that determines
        /// how the Answers/Option will be display in the Quiz Engines
        /// </summary>
        public int TypeId { get; set; }
        public string Type { get; set; }
        public Guid QuizId { get; set; }
        public string QuizName { get; set; }
        public int? Score { get; set; }
        public int? NumberOfOption { get; set; }
        public Guid CompanyId { get; set; }

        public virtual Quiz Quiz { get; set; }
        public virtual ICollection<QuestionOption> QuestionOptions { get; set; }

    }
}
