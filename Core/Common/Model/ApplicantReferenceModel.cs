using HRShared.Common;

namespace Core.Common.Model
{
    public class ApplicantReferenceModel
    {

    }


    public class ApplicantReferenceResponse : BaseResponseModel
    {

        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string Profession { get; set; }
        public string PlaceOfWork { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public Guid JobApplicantId { get; set; }
        public string? Address { get; set; }
    }

    public class UpdateApplicantReferenceRequest
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string Profession { get; set; }
        public string PlaceOfWork { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public Guid JobApplicantId { get; set; }
        public string? Address { get; set; }
    }

    public class ApplicantReferenceRequest
    {
        public Guid JobApplicantId { get; set; }
        public string FullName { get; set; }
        public string Profession { get; set; }
        public string PlaceOfWork { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string? Address { get; set; }
    }



    public class ReferenceListRequest : PaginationRequest
    {

        public Guid ApplicantId { get; set; }


    }

    // public class AllProfileListRequest
    // {
    //
    //     public string Name { get; set; }
    //
    //     public string WorkEmail { get; set; }
    //
    //
    //     public DateTime StartDate { get; set; }
    //
    //     public DateTime EndDate { get; set; }
    //
    // }

    public class ApplicantReferenceListResponse : BaseResponseModel
    {
        public Guid UserId { get; set; }
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string Profession { get; set; }
        public string PlaceOfWork { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public Guid JobApplicantId { get; set; }
        public Guid CompanyId { get; set; }
        public string? Address { get; set; }
    }
}