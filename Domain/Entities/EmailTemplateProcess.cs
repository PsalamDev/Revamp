using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class EmailTemplateProcess : BaseEntity
    {
        public string Email { get; set; }
        public Guid TemplateId { get; set; }
        public Guid UserId { get; set; }
        public string? Description { get; set; }
        public bool EmailSent { get; set; }
        public bool Processed { get; set; }
        public string? Status { get; set; }
    }
}
