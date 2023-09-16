using HRShared.Common;
using Microsoft.AspNetCore.Http;

namespace Core.Common.Model
{
    public class ApplicantDocumentModel
    {

    }

    public class ApplicantDocumentResponse : BaseResponseModel
    {
        public Guid ApplicantId { get; set; }
        public string FileType { get; set; }
        public string FileName { get; set; }
        public string Comment { get; set; }
        public string FileUrl { get; set; }
        public Guid DocuemntType { get; set; }
        public string? DocuemntTypeName { get; set; }
        public string DocumentTitle { get; set; }
    }

    public class ApplicantDocumentRequest
    {
        public Guid ApplicantId { get; set; }

        public IFormFile File { get; set; }

        public string Comment { get; set; }
        public Guid DocuemntType { get; set; }

        public string DocumentTitle { get; set; }

        public string? DocuemntTypeName { get; set; }
    }

    public class UpdateDocumentRequest
    {
        public Guid ApplicantId { get; set; }
        public IFormFile File { get; set; }
        public Guid DocuemntType { get; set; }

        public string DocumentTitle { get; set; }

        public string Comment { get; set; }
        public Guid Id { get; set; }
        public string? DocuemntTypeName { get; set; }
    }

    public class DocumentListRequest : PaginationRequest
    {

        public Guid ApplicantId { get; set; }


    }


    public class ApplicantDocumentListResponse : BaseResponseModel
    {

        public Guid Id { get; set; }
        public string FileType { get; set; }
        public string FileName { get; set; }
        public string Comment { get; set; }
        public string FileUrl { get; set; }
        public Guid ApplicantId { get; set; }

        public Guid DocuemntType { get; set; }
        public string? DocuemntTypeName { get; set; }
        public string DocumentTitle { get; set; }
    }
}