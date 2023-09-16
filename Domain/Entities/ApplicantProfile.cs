using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Index(propertyNames: nameof(PhoneNumber), IsUnique = true)]
    public class ApplicantProfile : BaseEntity
    {
        
        public Guid CompanyId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }
        public string Social { get; set; }
        public string Twitter { get; set; }
        public string Linkedin { get; set; }
        public string Instagram { get; set; }
        public string Address { get; set; }
        public string AgeRange { get; set; }
        public string PhoneNumber { get; set; }
        public string CountryCode { get; set; }

        public string? ProfileImage { get; set; }
        public Guid UserId { get; set; }

        public DateTime DateOfBirth { get; set; }

        public ICollection<JobPreference>? JobPreferences { get; set; } = new HashSet<JobPreference>();
        public ICollection<ApplicantSkill>? ApplicantSkills { get; set; } = new HashSet<ApplicantSkill>();
    }
}