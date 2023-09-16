using Core.Common.Model;
using HRShared.Common;

namespace Core.Interfaces
{
    public interface IApplicantWorkHistory
    {
        Task<ResponseModel<ApplicantHistoryResponse>> CreateAsync(ApplicantHistoryRequest request);
        Task<ResponseModel<ApplicantHistoryResponse>> UpdateAsync(UpdateApplicantHistoryRequest request);
        Task<ResponseModel<ApplicantHistoryResponse>> GetSingleAsync(Guid id);
        Task<ResponseModel<CustomPagination<List<ApplicantHistoryResponse>>>> GetAllListAsync(GetHistoryRequestList request);
        Task<ResponseModel<bool>> DeleteAsync(Guid id);
    }
}