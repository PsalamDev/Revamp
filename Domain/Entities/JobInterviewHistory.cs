using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class JobInterviewHistory : BaseEntity
    {
        public bool IsActive { get; set; } = false;
        public Guid? HireStageId { get; set; }

        public Guid? SubHireStageId { get; set; }

        public string StatusDescription { get; set; }
        public Guid JobApplicantId { get; set; }

        [Required(ErrorMessage = "JobApplication Id is required")]
        public Guid JobApplicationId { get; set; }

        [ForeignKey("JobApplicationId")]
        public JobApplication JobApplication { get; set; }
        public Guid CompanyId { get; set; }
    }
}
