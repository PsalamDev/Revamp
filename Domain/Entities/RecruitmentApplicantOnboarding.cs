using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class RecruitmentApplicantOnboarding : BaseEntity
    {
        public string OnboardingId { get; set; }
        public Guid JobApplicationId { get; set; }
        public Guid CompanyId { get; set; }
        public string IsOfferSent { get; set; }
    }
}
