using Core.Common.Model;
using HRShared.Common;

namespace Core.Interfaces
{
    public interface IApplicantQualification
    {
        Task<ResponseModel<ApplicantQualificationResponse>> CreateAsync(ApplicantQualificationRequest request);
        Task<ResponseModel<ApplicantQualificationResponse>> UpdateAsync(UpdateApplicantQualificationRequest request);
        Task<ResponseModel<ApplicantQualificationResponse>> GetSingleAsync(Guid id);
        Task<ResponseModel<CustomPagination<List<ApplicantQualificationListResponse>>>> GetAllListAsync(QualificationListRequest req);
        Task<ResponseModel<bool>> DeleteAsync(Guid id);
    }
}