using HRShared.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Common.Model.IdentityModels.Identity
{
    public class UpdateCompanyProfileModel : IRequest<ResponseModel<CompanyModel>>
    {
        public Guid? Id { get; set; }
        public string? PhoneNumber { get; set; }
        public string? RcNumber { get; set; }
        public string? CompanyEmail { get; set; }
        public string? DomainName { get; set; }
        public string? AuditorsEmail { get; set; }
        public string? AdministratorEmail { get; set; }
        public int? MaxEmployeeCount { get; set; }
        public string? CompanyAddress { get; set; }
        public string? AboutCompany { get; set; }

        public bool Deactivated { get; set; }
    }

}
