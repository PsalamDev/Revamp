using Core.Interfaces;
using HRShared.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Domain.Enums.Enum;

namespace Core.Common.Model
{
    public class JobPreferenceModel : BaseResponseModel
    {
        public string JobTitle { get; set; }
        public EmploymentType EmploymentType { get; set; }
        public decimal? SalaryRangeFrom { get; set; }
        public decimal? SalaryRangeTo { get; set; }
        public Guid ApplicantprofileId { get; set; }
        public Guid CompanyId { get; set; }
        public string? Experiencelevel { get; set; }
    }

    public class CreateJobPreferenceDto
    {
        public Guid ApplicantId { get; set; }
        public string JobTitle { get; set; }
        public EmploymentType EmploymentType { get; set; }
        public decimal? SalaryRangeFrom { get; set; }
        public decimal? SalaryRangeTo { get; set; }
        public string? Experiencelevel { get; set; }
    }

    public class UpdateJobPreferencDto
    {
        public Guid Id { get; set; }
        public string JobTitle { get; set; }
        public EmploymentType EmploymentType { get; set; }
        public decimal? SalaryRangeFrom { get; set; }
        public decimal? SalaryRangeTo { get; set; }
        public string? Experiencelevel { get; set; }
    }

    public class JobPreferenceDto
    {
        public Guid Id { get; set; }
        public string JobTitle { get; set; }
        public EmploymentType EmploymentType { get; set; }
        public string EmploymentTypeText { get; set; }
        public Guid ApplicantprofileId { get; set; }
        public Guid CompanyId { get; set; }
        public decimal? SalaryRangeFrom { get; set; }
        public decimal? SalaryRangeTo { get; set; }
        public string? Experiencelevel { get; set; }
    }
    public class JobPreferenceFilter : PaginationRequest
    {
        public Guid ApplicantId { get; set; }
        public decimal? SalaryRangeFrom { get; set; }
        public decimal? SalaryRangeTo { get; set; }
        public string? Experiencelevel { get; set; }
    }
}
