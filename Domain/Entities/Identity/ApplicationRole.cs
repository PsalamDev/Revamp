using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Identity
{
    public class ApplicationRole : IdentityRole
    {
        public ApplicationRole()
        {
        }
        public Guid CompanyId { get; set; }
        public string? Description { get; set; }

        public ApplicationRole(string roleName, string? description = null)
            : base(roleName)
        {
            Description = description;
            NormalizedName = roleName.ToUpperInvariant();
        }
    }

}