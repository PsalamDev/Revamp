using Core.Common.Model;
using HRShared.Common;

namespace Core.Interfaces
{
    public interface IApplicantDocumentService
    {
        Task<ResponseModel<ApplicantDocumentResponse>> CreateAsync(ApplicantDocumentRequest request);
        Task<ResponseModel<ApplicantDocumentResponse>> UpdateAsync(UpdateDocumentRequest request);
        Task<ResponseModel<ApplicantDocumentResponse>> GetSingleAsync(Guid id);
        Task<ResponseModel<CustomPagination<List<ApplicantDocumentListResponse>>>> GetAllListAsync(DocumentListRequest model);
        Task<ResponseModel<bool>> DeleteAsync(Guid id);
    }
}