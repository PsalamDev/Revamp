
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence.Initialization
{
    internal class ApplicationDbInitializer
    {
        private readonly ApplicationDbContext _dbContext;
        //private readonly ITenantInfo _currentTenant;
        private readonly ApplicationSeeder _dbSeeder;
        private readonly ILogger<ApplicationDbInitializer> _logger;

        public ApplicationDbInitializer(ApplicationDbContext dbContext,
            //ITenantInfo currentTenant,
            ILogger<ApplicationDbInitializer> logger, ApplicationSeeder dbSeeder)
        {
            _dbContext = dbContext;
            //  _currentTenant = currentTenant;
            _dbSeeder = dbSeeder;
            _logger = logger;
        }

        public async Task InitializeAsync(CancellationToken cancellationToken, bool reload = true)
        {
            if (_dbContext.Database.GetMigrations().Any())
            {
                if (_dbContext.Database.GetPendingMigrations().Any())
                {
                    _logger.LogInformation("Applying Migrations for  tenant.");
                    await _dbContext.Database.MigrateAsync(cancellationToken);
                }

                if (_dbContext.Database.CanConnect())
                {
                    _logger.LogInformation("Connection to  Database Succeeded.");

                    await _dbSeeder.SeedDatabase(_dbContext, cancellationToken, reload);
                }
            }
        }
    }
}
