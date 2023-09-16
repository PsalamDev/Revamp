using Infrastructure.Persistence.Initialization;
using Infrastructure.Persistence.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Persistence.Initialization
{
    internal class DatabaseInitializer : IDatabaseInitializer
    {
        // private readonly TenantDbContext _tenantDbContext;
        private readonly DatabaseSettings _dbSettings;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DatabaseInitializer> _logger;

        public DatabaseInitializer(
            // TenantDbContext tenantDbContext, 
            IOptions<DatabaseSettings> dbSettings, IServiceProvider serviceProvider, ILogger<DatabaseInitializer> logger)
        {
            //  _tenantDbContext = tenantDbContext;
            _dbSettings = dbSettings.Value;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task InitializeDatabasesAsync(CancellationToken cancellationToken)
        {
            //await InitializeTenantDbAsync(cancellationToken);


            await InitializeApplicationDbForTenantAsync(cancellationToken);


        }

        public async Task InitializeApplicationDbForTenantAsync(CancellationToken cancellationToken, bool reload = true)
        {
            // First create a new scope
            using var scope = _serviceProvider.CreateScope();

            // Then run the initialization in the new scope
            await scope.ServiceProvider.GetRequiredService<ApplicationDbInitializer>()
                .InitializeAsync(cancellationToken, reload);
        }


        private async Task InitializeTenantDbAsync(CancellationToken cancellationToken)
        {
            //  if (_tenantDbContext.Database.GetPendingMigrations().Any())
            //  {
            //       _logger.LogInformation("Applying Root Migrations.");
            //     await _tenantDbContext.Database.MigrateAsync(cancellationToken);
            // }


            //     await SeedRootTenantAsync(cancellationToken);
        }

        /* private async Task SeedRootTenantAsync(CancellationToken cancellationToken)
         {
             if (await _tenantDbContext.TenantInfo.FindAsync(new object?[] { MultiTenantConstant.Root.Id }, cancellationToken: cancellationToken) is null)
             {
                 var rootTenant = new ExTenantInfo(
                     MultiTenantConstant.Root.Id,
                     MultiTenantConstant.Root.Name,
                     _dbSettings.DefaultConnection,
                     MultiTenantConstant.Root.EmailAddress);

                 rootTenant.SetValidity(DateTime.UtcNow.AddYears(1));

                 _tenantDbContext.TenantInfo.Add(rootTenant);

                 await _tenantDbContext.SaveChangesAsync(cancellationToken);
             }
         }
         */
    }
}
