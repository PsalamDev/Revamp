using HRShared.Common;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Common.Model
{
    public class CompanyModel : BaseResponseModel
    {

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

        public bool Deactivated { get; set; }

    }
    public class CreateCompanyCommand : IRequest<ResponseModel<CompanyModel>>
    {
        public string NameOfOrganization { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;
        public string Password { get; set; } = default!;
    }

    public class AddCompanyProfileImageCommand : IRequest<ResponseModel<CompanyModel>>
    {
        public IFormFile? File { get; set; }
        public Guid companyId { get; set; }
    }

    public class CompanyQueryModel : BaseQueryModel
    {

    }
}
