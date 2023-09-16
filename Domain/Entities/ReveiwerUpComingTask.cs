using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ReveiwerUpComingTask : BaseEntity
    {
        public string UpComingTaskDescription { get; set; }
        public Guid CompanyId { get; set; }
        public DateTime UpComingTaskDate { get; set; }
        public Guid? InterviewerEmployeeId { get; set; }
    }
}
