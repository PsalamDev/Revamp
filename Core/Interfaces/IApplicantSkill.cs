using Core.Common.Model;
using HRShared.Common;

namespace Core.Interfaces
{
    public interface IApplicantSkill
    {
        Task<ResponseModel<string>> CreateAsync(ApplicantSkillRequest request);
        Task<ResponseModel<string>> UpdateAsync(UpdateApplicantSkillRequest request);
        Task<ResponseModel<List<ApplicantSkillResponse>>> GetListAsync(ApplicantSkillFilter filter);
        Task<ResponseModel<bool>> DeleteAsync(Guid id);
        Task<ResponseModel<List<ApplicantSkillNameSuggestion>>> GetSkillNameSuggestions();
    }
}