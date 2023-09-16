using Domain.Interfaces;
using Infrastructure.Persistence.Context;
using Infrastructure.Persistence.Initialization;
using Infrastructure.Persistence.Repository;
using Infrastructure.Persistence.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Persistence
{
    public static class Startup
    {
        internal static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration config)
        {
            // TODO: there must be a cleaner way to do IOptions validation...
            var rootConnectionString = config.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(rootConnectionString))
            {
                throw new InvalidOperationException("DB ConnectionString is not configured.");
            }


            return services
                .Configure<DatabaseSettings>(config.GetSection("DefaultConnection"))

                .AddDbContext<ApplicationDbContext>(m => m.UseDatabase(rootConnectionString))

                .AddDbContext<ApplicationDbContext>(m => m.UseDatabase(rootConnectionString))

                .AddTransient<IDatabaseInitializer, DatabaseInitializer>()
                .AddTransient<ApplicationDbInitializer>()
                .AddRepositories()
                .AddTransient<ApplicationSeeder>()
                ;

        }

        internal static DbContextOptionsBuilder UseDatabase(this DbContextOptionsBuilder builder, string connectionString)
        {
            var assemblyName = typeof(ApplicationDbContext).AssemblyQualifiedName;
            return builder.UseSqlServer(connectionString);
        }


        private static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            // Add Repositories
            services.AddScoped(typeof(IAsyncRepository<,>), typeof(EfRepository<,>));

            /* foreach (var aggregateRootType in
                 typeof(IAggregateRoot).Assembly.GetExportedTypes()
                     .Where(t => typeof(IAggregateRoot).IsAssignableFrom(t) && t.IsClass)
                     .ToList())
             {
             }
 */
            return services;
        }
    }
}