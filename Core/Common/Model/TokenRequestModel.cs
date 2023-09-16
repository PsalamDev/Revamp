using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Common.Model
{
    public class TokenRequestModel
    {
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string IPAddress { get; set; } = default!;
    }

    public class TokenResponseModel
    {
        public string AccessToken { get; set; } = default!;
        public string RefereshToken { get; set; } = default!;
        public CompanyModel? Company { get; set; }
        public string UserId { get; set; } = default!;
        public CompanyPlanTokenResponseModel? CompanyPlan { get; set; }
    }

    public class CompanyPlanTokenResponseModel
    {
        public Guid PlanId { get; set; } = default!;
        public int? ExtraNoOfEmployement { get; set; }
        public decimal Amount { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public bool IsPaid { get; set; }
        public bool IsRecurring { get; set; }
        public bool IsActive { get; set; }
        public bool IsCancelled { get; set; }

    }
}
