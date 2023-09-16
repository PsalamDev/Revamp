
using AutoMapper;
using Core.Common.Model;
using Domain.Entities;
using System.Reflection;

namespace Infrastructure.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            Config();
            ApplyMappingsFromAssembly(Assembly.GetExecutingAssembly());
        }

        private void ApplyMappingsFromAssembly(Assembly assembly)
        {
            var types = assembly.GetExportedTypes()
                .Where(t => t.GetInterfaces().Any(i =>
                    i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMapFrom<>)))
                .ToList();

            foreach (var type in types)
            {
                var instance = Activator.CreateInstance(type);

                var methodInfo = type.GetMethod("Mapping")
                    ?? type.GetInterface("IMapFrom`1").GetMethod("Mapping");

                methodInfo?.Invoke(instance, new object[] { this });
            }
        }


        private void Config()
        {

            CreateMap<Stage, StageModel>().ReverseMap();
            CreateMap<Stage, StagesModel>().ReverseMap();
            CreateMap<Stage, CreateStageRequestModel>().ReverseMap();
            CreateMap<Stage, UpdateStageRequestModel>().ReverseMap();
            CreateMap<SubStage, SubStageModel>().ReverseMap();
            CreateMap<SubStage, SubStagesModel>().ReverseMap();
            CreateMap<SubStage, CreateSubStageRequestModel>().ReverseMap();
            CreateMap<SubStage, UpdateSubStageRequestModel>().ReverseMap();
            CreateMap<ScoreCard, CreateScoreCardRequestModel>().ReverseMap();
            CreateMap<ScoreCard, UpdateScoreCardRequestModel>().ReverseMap();
            CreateMap<ScoreCard, ScoreCardModel>().ReverseMap();
            CreateMap<JobReviewer, JobReviewerModel>().ReverseMap();
            CreateMap<ScoreCardQuestion, ScoreCardQuestionModel>().ReverseMap();
            CreateMap<ScoreCardQuestion, CreateScoreCardQuestionModel>().ReverseMap();
            CreateMap<ScoreCardQuestion, UpdateScoreCardQuestionModel>().ReverseMap();
            CreateMap<RecruitmentFocusArea, RecruitmentFocusAreaModel>().ReverseMap();
            CreateMap<RecruitmentFocusArea, CreateRecruitmentFocusAreaRequestModel>().ReverseMap();
            CreateMap<RecruitmentFocusArea, UpdateRecruitmentFocusAreaRequestModel>().ReverseMap();

            CreateMap<ApplicantHistoryResponse, ApplicantWorkHistory>().ReverseMap();
            CreateMap<ApplicantQualificationResponse, ApplicantEducationHistory>().ReverseMap();
            CreateMap<ApplicantReferenceResponse, ApplicantReference>().ReverseMap();
            CreateMap<ApplicantSkillResponse, ApplicantSkill>().ReverseMap();
            CreateMap<ApplicantDocumentResponse, ApplicantDocument>().ReverseMap();
            CreateMap<ApplicantProfileResponse, ApplicantProfile>().ReverseMap();

            CreateMap<Job, JobModel>().ReverseMap();
            CreateMap<Job, UpdateJobDto>().ReverseMap();
            CreateMap<Job, CreateJobDto>().ReverseMap();


            CreateMap<JobPreference, CreateJobPreferenceDto>().ReverseMap();
            CreateMap<JobPreference, JobPreferenceModel>().ReverseMap();
            CreateMap<JobPreference, UpdateJobPreferencDto>().ReverseMap();
        }
    }
}
