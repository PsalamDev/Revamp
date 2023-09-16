using Core.Common.Model;
using Core.Common.Model.RecruitmentDto;
using HRShared.Common;

namespace Core.Interfaces
{
    public interface IApplicantProfile
    {
        Task<ResponseModel<ApplicantProfileResponse>> CreateAsync(ApplicantProfileRequest request);
        Task<ResponseModel<ApplicantProfileResponse>> UploadApplicantProfileImage(ApplicantProfileImage request);
        Task<ResponseModel<ApplicantProfileResponse>> UpdateAsync(UpdateApplicantProfileRequest request);
        Task<ResponseModel<ApplicantProfileResponse>> GetSingleAsync(Guid id);
        Task<ResponseModel<ApplicantProfileResponse>> GetSingleByEmailAsync(string email);
        Task<ResponseModel<CustomPagination<List<ApplicantProfileListResponse>>>> GetAllListAsync(ProfileListRequest request);
        Task<ResponseModel<DocumentResponseDTO>> DownloadApplicantCV(Guid applicantId);
        Task<ResponseModel<ApplicantProfileFullDetailsResponse>> GetCompleteSingleApplicantProfileAsync(Guid id);
        Task<ResponseModel<bool>> DeleteApplicanKeywordAsync(Guid id);
        Task<ResponseModel<List<JobWordDto>>> LoadApplicantKeyword(Guid applicantId);
    }
}