using HRShared.Common;

namespace Core.Common.Model
{
    public class ApplicantHistoryModel
    {

    }

    public class ApplicantHistoryResponse : BaseResponseModel
    {
        public Guid Id { get; set; }
        public Guid ApplicantId { get; set; }
        public string CompanyName { get; set; }
        public string JobTitle { get; set; }
        public string Department { get; set; }
        public string Grade { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? JobDescription { get; set; }
        public bool IsCurrent { get; set; }
    }

    public class ApplicantHistoryRequest
    {
        public Guid ApplicantId { get; set; }
        public string CompanyName { get; set; }
        public string JobTitle { get; set; }
        public string? JobDescription { get; set; }
        public string Department { get; set; }
        public string Grade { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public bool IsCurrent { get; set; }
    }

    public class UpdateApplicantHistoryRequest
    {
        public Guid Id { get; set; }
        public Guid ApplicantId { get; set; }
        public string CompanyName { get; set; }
        public string JobTitle { get; set; }
        public string? JobDescription { get; set; }
        public string Department { get; set; }
        public string Grade { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public bool IsCurrent { get; set; }
    }

    public class GetApplicantHistory : PaginationRequest
    {
        public Guid ApplicantId { get; set; }
    }

    public class GetHistoryRequestList : PaginationRequest
    {
        public Guid ApplcantId { get; set; }
    }
}