using Azure.Storage.Blobs;
using Core.Common.Settings;
using Core.Interfaces;
using FluentValidation;
using HRShared.CoreProviders.Implementation;
using HRShared.CoreProviders.Interfaces;
using Infrastructure.Implementation;
using Infrastructure.Persistence.Context;
using Infrastructure.Providers.Implementation;
using Infrastructure.Providers.Interface;
//using Infrastructure.Implementation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;
using Serilog.Sinks.MSSqlServer;
using System.Reflection;

namespace Infrastructure
{
    public static class DependencyInjection
    {
       
        
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddMediatR(Assembly.GetExecutingAssembly());

            #region pipeline

            /*
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));

            */

            #endregion

            services.AddSingleton<IServiceProvider>(sp => sp);
            services.AddHttpContextAccessor();

            //services.AddScoped<ExceptionMiddleware>();
            services.AddTransient<ITokenService, TokenService>();
            services.AddTransient<IIdentityService, IdentityService>();
            services.AddTransient<IClaimsService, ClaimsService>();
            services.AddTransient<IRoleClaimsService, RoleClaimsService>();
            services.AddTransient<IUserRoleService, UserRoleService>();
            services.AddTransient<IRoleService, RoleService>();



            services.AddTransient<IHiringStageService, HiringStageService>();
            services.AddTransient<IHiringSubStageService, HiringSubStageService>();
            services.AddTransient<IQuizService, QuizService>();
            services.AddTransient<IApplicantProfile, ApplicantProfileService>();
            services.AddTransient<IJobPreferenceService, JobPreferenceService>();
            services.AddTransient<IMailService, MailService>();

            services.AddTransient<IScoreCardQuestionService, ScoreCardQuestionService>();
            services.AddTransient<IScoreCardService, ScoreCardService>();
            services.AddTransient<IRecruitmentFocusAreaService, RecruitmentFocusAreaService>();
            services.AddTransient<IAzureStorageServices, AzureStorageServices>();
            services.AddTransient<IApplicantQualification, ApplicantQualificationService>();
            services.AddTransient<IApplicantWorkHistory, ApplicantWorkHistoryService>();
            services.AddTransient<IApplicantDocumentService, ApplicantDocumentService>();
            services.AddTransient<IApplicantReference, ApplicantReferenceService>();
            services.AddTransient<IApplicantSkill, ApplicantSkillService>();
            services.AddTransient<IJobService, JobService>();
            services.AddTransient<IHiringStageService, HiringStageService>();
            services.AddTransient<IHiringSubStageService, HiringSubStageService>();
            services.AddTransient<IQuizService, QuizService>();
            //services.AddTransient<IMailService, MailService>();
            services.AddTransient<IRecruitmentJobApplicationServices, RecruitmentJobApplicationServices>();
            services.AddHttpClient();

         //   services.AddTransient<BackgroundJobService>();
            // services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            return services;
        }



        public static void AddAzureStorageService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped(options =>
            {
                return new BlobServiceClient(configuration.GetConnectionString("AzureStorageConnectionString"));
            });
        }

        public static void AddSettingsConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<EmailSettings>(configuration.GetSection(nameof(EmailSettings)));
            services.Configure<QuizSettings>(configuration.GetSection(nameof(QuizSettings)));
        }

        //public static void SerilogSettings(this IConfiguration configuration)
        //{
        //    Log.Logger = new LoggerConfiguration()
        //        .WriteTo.Console(new JsonFormatter())
        //        .WriteTo.MSSqlServer("Data Source=54.155.204.222;Initial Catalog=HrSystemLogs;Persist Security Info=False;User Id=sa;Password=3nFF6@5raYOf3;Encrypt=false;TrustServerCertificate=true",
        //                             new MSSqlServerSinkOptions
        //                             {
        //                                 TableName = "HRRecruitmentLog",
        //                                 SchemaName = "hrsystemslog",
        //                                 AutoCreateSqlTable = true
        //                             })
        //        .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
        //        .CreateLogger();

        //}
    }
}