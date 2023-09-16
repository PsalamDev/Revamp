
using HRShared.Common;
using Infrastructure.Persistence.Context;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence.Initialization
{
    public class ApplicationSeeder
    {
        private readonly ILogger<ApplicationSeeder> _logger;

        public ApplicationSeeder(

            ILogger<ApplicationSeeder> logger)
        {

            _logger = logger;
        }

        public async Task SeedDatabase(ApplicationDbContext context, CancellationToken cancellation, bool reload = true)
        {
            if (reload)
            {
                var id = SequentialGuid.Create();
                await SeedPermissions(context, id);

                await SeedRoles(context, id);

                //     await SeedDefaultCompanyandUser(context,id);
            }


        }


        private async Task SeedPermissions(ApplicationDbContext context, Guid id)
        {
            //seed super admins permissions
            // var superAdminPermissions = DefaultPermissionsHelper.SuperPermissionTypes;
            //  var adminPermission = DefaultPermissionsHelper.AdminPermissionTypes;
            //  var currentPermissions = await context.Permissions.ToListAsync();

            //  await HandlePermissionSeeding(superAdminPermissions, context, currentPermissions, true);
            //  await HandlePermissionSeeding(adminPermission, context, currentPermissions, null);

        }


        private async Task SeedRoles(ApplicationDbContext context, Guid id)
        {
            // var shadow = _currentTenant.ConnectionString;


            /*
            foreach (string roleName in RoleService.DefaultRoles)
             {
                 var role = await _roleManager.Roles.SingleOrDefaultAsync(r => r.Name == roleName);

                 if (role == null)
                 {
                     // Create the role
                     role = new ApplicationRole(roleName, $"{roleName} Role for default Tenant");
                     _logger.LogInformation("Seeding {role} Role for default Tenant.", roleName);
                     role.CompanyId = id;
                     await _roleManager.CreateAsync(role);
                 }

                 // Assign permissions
                 var allPermissions = await context.Permissions.ToListAsync();

                 var roleClaims = await context.RoleClaims.Where(x => x.RoleId == role.Id).ToListAsync();

                 allPermissions = allPermissions.Where(p => p.Id != roleClaims.Where(r => r.ClaimId == p.Id).Select(s => s.ClaimId).FirstOrDefault()).ToList();

                 if (roleName == DefaultRoleConstant.SuperAdmin)
                 {
                     //get all claims for this role
                     //filter out the ones that are currently not in the role
                     //add it to the permission
                     await AssignRolePermissions(allPermissions, context, role,id);
                 }
                 else if (roleName == DefaultRoleConstant.Admin)
                 {
                     allPermissions = allPermissions.Where(x => x.IsAdmin == null || !x.IsAdmin.Value ).ToList();
                     await AssignRolePermissions(allPermissions, context, role,id);
                 }
             }
 */
        }

    }
}
