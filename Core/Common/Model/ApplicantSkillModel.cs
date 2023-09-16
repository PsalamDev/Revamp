using HRShared.Common;

namespace Core.Common.Model
{
    public class ApplicantSkillModel
    {

    }

    public class ApplicantSkillResponse : BaseResponseModel
    {
        public Guid Id { get; set; }
        public Guid ApplicantId { get; set; }
        public Guid CompanyId { get; set; }
        public string SkillName { get; set; }
    }

    public class ApplicantSkillRequest
    {
        public Guid ApplicantId { get; set; }
        public List<string> SkillNames { get; set; }
    }

    public class UpdateApplicantSkillRequest
    {
        public Guid Id { get; set; }
        public Guid ApplicantId { get; set; }
        public List<string> SkillNames { get; set; }

    }
    public class ApplicantSkillFilter
    {
        public Guid ApplicantId { get; set; }

    }

    public class ApplicantSkillNameSuggestion
    {
        public string SkillNames { get; set; }

    }
}