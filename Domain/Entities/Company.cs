using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Company
    {
      

        public Guid Id { get; set; }
        public Guid CreatedBy { get; set; }

        public string? CreatedByIp { get; set; }

        public DateTime CreatedDate { get; set; }

        public string? ModifiedBy { get; set; }

        public string? ModifiedByIp { get; set; }

        public DateTime? ModifiedDate { get; set; }
        public bool IsDeleted { get; set; } = false;
        public string? NameOfOrganization { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? RcNumber { get; set; }
        public string? CompanyEmail { get; set; }
        public string? DomainName { get; set; }
        public string? AuditorsEmail { get; set; }
        public string? AdministratorEmail { get; set; }
        public int? MaxEmployeeCount { get; set; }
        public string? CompanyAddress { get; set; }
        public string? AboutCompany { get; set; }
        public string? CompanyProfileImageUrl { get; set; }
        public bool IsProfileUpdated { get; set; }
        public bool IsEmailVerified { get; set; }
        public bool HasPlan { get; set; }
        public bool? Deactivated { get; set; }

    }
}
