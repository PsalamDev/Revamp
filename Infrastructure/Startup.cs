using Coravel;
using Coravel.Scheduling.Schedule;
using Google.Protobuf.WellKnownTypes;
using Infrastructure.Extensions;
using Infrastructure.OpenApi;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Initialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class Startup
    {

        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {

            return services
                .AddApiVersioning()
                .AddAuth(config)
                //.AddDbContext<TenantDbContext>(m => m.UseDatabase(config.GetConnectionString("DefaultConnection")))
                .AddOpenApiDocumentation(config)
                .AddPersistence(config)

                .AddRouting(options => options.LowercaseUrls = true)
                .AddServices()
                .AddScheduler()
                //.AddBackgroundJobs(config)
                ;

        }

        // internal static IApplicationBuilder UseExceptionMiddleware(this IApplicationBuilder app) =>
        // app.UseMiddleware<ExceptionMiddleware>();




        private static IServiceCollection AddApiVersioning(this IServiceCollection services) =>
            services.AddApiVersioning(config =>
            {
                config.ReportApiVersions = true;
                config.AssumeDefaultVersionWhenUnspecified = true;
                config.DefaultApiVersion = new ApiVersion(1, 0);
            });


        public static async Task InitializeDatabasesAsync(this IServiceProvider services, CancellationToken cancellationToken = default)
        {
            // Create a new scope to retrieve scoped services
            using var scope = services.CreateScope();

            await scope.ServiceProvider.GetRequiredService<IDatabaseInitializer>()
                .InitializeDatabasesAsync(cancellationToken);
        }

        public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder builder, IConfiguration config) =>
            builder
                // .UseStaticFiles()
                // .UseRouting()
                // .UseCors("corsapp")
                // .UseAuthentication()
                .UseCurrentUser()
                .UseOpenApiDocumentation(config)

            //  .UseAuthorization()
            //   .UseEndpoints(config => config.MapControllers())
            //.UseHangfireDashboard(config)
            // .CallRecurringJobs()

            ;


        //        provider.UseScheduler(scheduler =>
        //{
        //    scheduler.Schedule(
        //        () => Console.WriteLine("Every minute during the week.")
        //    )
        //    .EveryMinute()
        //    .Weekday();
        //    });


        public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder builder)
        {
            builder.MapControllers().RequireAuthorization();
            return builder;
        }

        private static IEndpointConventionBuilder MapHealthCheck(this IEndpointRouteBuilder endpoints) =>
            endpoints.MapHealthChecks("/api/health").RequireAuthorization();
    }
}