using HRShared.Common;
using Microsoft.AspNetCore.Http;

namespace Core.Common.Model
{
    public class ApplicantProfileModel
    {

    }


    public class ApplicantProfileResponse : BaseResponseModel
    {
        public Guid UserId { get; set; }
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? PersonalEmail { get; set; }
        public string? CountryCode { get; set; }
        public string PhoneNumber { get; set; }
        public string DateOfBirth { get; set; }
        public int MaritalStatus { get; set; }
        public int Gender { get; set; }
        public string? ProfileImage { get; set; }
        public string Twitter { get; set; }
        public string Linkedin { get; set; }
        public string Instagram { get; set; }
        public string Address { get; set; }
        public string AgeRange { get; set; }
        public string Website { get; set; }
        public string Email { get; set; }
    }

    public class UpdateApplicantProfileRequest
    {
        public Guid UserId { get; set; }
        public Guid ApplicantId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? CountryCode { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Twitter { get; set; }
        public string Linkedin { get; set; }
        public string Instagram { get; set; }
        public string Address { get; set; }
        public string AgeRange { get; set; }
        public int MaritalStatus { get; set; }
        public int Gender { get; set; }
        public string? ProfileImage { get; set; }
        public Guid Id { get; set; }
        public string? Website { get; set; }
    }

    public class ApplicantProfileRequest
    {
        public Guid UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? PersonalEmail { get; set; }
        public string? CountryCode { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime DateOfBirth { get; set; }

        public string Password { get; set; }
        public string Twitter { get; set; }
        public string Linkedin { get; set; }
        public string Instagram { get; set; }
        public string Address { get; set; }
        public string AgeRange { get; set; }

        public int MaritalStatus { get; set; }
        public int Gender { get; set; }
        public Guid CompanyId { get; set; }
        public string? Website { get; set; }

    }

    public class ApplicantProfileImage
    {

        public IFormFile image { get; set; }

        public Guid ApplicantId { get; set; }
    }

    public class ProfileListRequest : PaginationRequest
    {

        public string? Name { get; set; }


        public string? Email { get; set; }


    }

    public class AllProfileListRequest
    {

        public string Name { get; set; }

        public string WorkEmail { get; set; }


        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

    }

    public class ApplicantProfileListResponse : BaseResponseModel
    {
        public Guid UserId { get; set; }
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? PersonalEmail { get; set; }
        public string? CountryCode { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int MaritalStatus { get; set; }
        public int Gender { get; set; }
        public string? ProfileImage { get; set; }
    }

    public class DocumentResponseDTO
    {
        public string FileName { get; set; }
        public MemoryStream TemplateFile { get; set; }
        public string FilePath { get; set; }
        public string ContentType { get; set; }
    }

    public class ApplicantProfileFullDetailsResponse
    {
        public Guid UserId { get; set; }
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? PersonalEmail { get; set; }
        public string? CountryCode { get; set; }
        public string PhoneNumber { get; set; }
        public string DateOfBirth { get; set; }
        public int MaritalStatus { get; set; }
        public int Gender { get; set; }
        public string? ProfileImage { get; set; }
        public string Twitter { get; set; }
        public string Linkedin { get; set; }
        public string Instagram { get; set; }
        public string Address { get; set; }
        public string AgeRange { get; set; }
        public string Website { get; set; }
        public string Email { get; set; }
    }


    public class ApplicantCV
    {

        public IFormFile CV { get; set; }

        public Guid ApplicantId { get; set; }
    }

    public class CreateJobWordDto
    {

        public Guid applicantId { get; set; }

        public string AppKeyword { get; set; }
    }

    public class JobWordDto
    {
        public Guid Id { get; set; }
        public Guid ApplicantId { get; set; }
        public string AppKeyword { get; set; }
    }
}