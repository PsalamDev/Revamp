using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Permission : BaseEntity
    {
        public string ClaimName { get; set; } = default!;
        public string ClaimValue { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string Group { get; set; } = default!;
        public bool? IsAdmin { get; set; } = default!;
    }
}
