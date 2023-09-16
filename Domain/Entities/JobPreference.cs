using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Domain.Enums.Enum;

namespace Domain.Entities
{
    public class JobPreference : BaseEntity
    {
        public string JobTitle { get; set; }
        public EmploymentType EmploymentType { get; set; }
        public Guid ApplicantprofileId { get; set; }
        public ApplicantProfile ApplicantProfile { get; set; }
        public Guid CompanyId { get; set; }
        public decimal? SalaryRangeFrom { get; set; }
        public decimal? SalaryRangeTo { get; set; }
        public string? Experiencelevel { get; set; }
    }
}
