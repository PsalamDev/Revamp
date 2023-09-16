using Core.Common.Model;
using Domain.Shared;
using HRShared.Common;

namespace Core.Interfaces
{
    public interface IApplicantReference
    {
        Task<ResponseModel<ApplicantReferenceResponse>> CreateAsync(ApplicantReferenceRequest request);
        Task<ResponseModel<ApplicantReferenceResponse>> UpdateAsync(UpdateApplicantReferenceRequest request);
        Task<ResponseModel<ApplicantReferenceResponse>> GetSingleAsync(Guid id);
        Task<ResponseModel<CustomPagination<List<ApplicantReferenceListResponse>>>> GetAllListAsync(ReferenceListRequest model);
        Task<ResponseModel<bool>> DeleteAsync(Guid id);
    }
}