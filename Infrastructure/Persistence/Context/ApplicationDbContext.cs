using Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Domain.Entities.Identity;

namespace Infrastructure.Persistence.Context
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string, IdentityUserClaim<string>, IdentityUserRole<string>, IdentityUserLogin<string>, ApplicationRoleClaim, IdentityUserToken<string>>
    {

        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Stage> Stages => Set<Stage>();
        public DbSet<ScoreCard> ScoreCards => Set<ScoreCard>();
        public DbSet<SubStage> SubStages => Set<SubStage>();
        public DbSet<Question> Questions => Set<Question>();  
        public DbSet<Company> Companies => Set<Company>();  
        public DbSet<Permission> Permissions => Set<Permission>(); 
        public DbSet<Quiz> Quizzes => Set<Quiz>();
        public DbSet<ApplicantAnswer> ApplicantAnswers => Set<ApplicantAnswer>();
        public DbSet<ApplicantQuizRecord> ApplicantQuizRecords => Set<ApplicantQuizRecord>();
        public DbSet<PsychometricType> PsychometricTypes => Set<PsychometricType>();
        public DbSet<QuestionOption> QuestionOptions => Set<QuestionOption>();
        public DbSet<ApplicantProfile> ApplicantProfiles => Set<ApplicantProfile>();
        public DbSet<ApplicantWorkHistory> ApplicantWorkHistories => Set<ApplicantWorkHistory>();
        public DbSet<ApplicantEducationHistory> ApplicantEducationHistories => Set<ApplicantEducationHistory>();
        public DbSet<ApplicantDocument> ApplicantDocuments => Set<ApplicantDocument>();
        public DbSet<ScoreCardQuestion> ScoreCardQuestions => Set<ScoreCardQuestion>();
        public DbSet<RecruitmentFocusArea> RecruitmentFocusAreas => Set<RecruitmentFocusArea>();
        public DbSet<ApplicantReference> ApplicantReferences => Set<ApplicantReference>();
        public DbSet<ApplicantSkill> ApplicantSkills => Set<ApplicantSkill>();
        public DbSet<Job> Jobs => Set<Job>();
        public DbSet<JobReviewer> JobReviewers => Set<JobReviewer>();
        public DbSet<JobApplication> JobApplications => Set<JobApplication>();
        public DbSet<JobInterviewHistory> JobInterviewHistories => Set<JobInterviewHistory>();
        public DbSet<AggregateScore> AggregateScores => Set<AggregateScore>();
        public DbSet<QuestionsWithMark> QuestionsWithMarks => Set<QuestionsWithMark>();
        public DbSet<JobScheduleInterview> JobScheduleInterviews => Set<JobScheduleInterview>();
        public DbSet<JobPreference> JobPreferences => Set<JobPreference>();
        public DbSet<JobApplicationStageHistory> JobApplicationStageHistories => Set<JobApplicationStageHistory>();
        public DbSet<ApplicantJobSearchKeyword> ApplicantJobSearchKeywords => Set<ApplicantJobSearchKeyword>();
        public DbSet<ReveiwerUpComingTask> ReveiwerUpComingTasks => Set<ReveiwerUpComingTask>();
        public DbSet<JobApplicantCVBank> JobApplicantCVBanks => Set<JobApplicantCVBank>();
        public DbSet<EmailTemplateProcess> EmailTemplateProcesses => Set<EmailTemplateProcess>();
        public DbSet<RecruitmentApplicantOnboarding> RecruitmentApplicantOnboardings => Set<RecruitmentApplicantOnboarding>();



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Not needed if db context derives from MultiTenantDbContext
            // builder.ConfigureMultiTenant();
            //modelBuilder.Entity<EmployeeCountRecord>(a =>
            //{
            //    a.HasNoKey();
            //});
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            //this.EnforceMultiTenant();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            //this.EnforceMultiTenant();
            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
    }
}