using Core.Constants;
using System.Security.Claims;

namespace Core.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string? GetEmail(this ClaimsPrincipal principal)
            => principal.FindFirstValue(ClaimTypes.Email);

        public static string? GetTenant(this ClaimsPrincipal principal)
            => principal.FindFirstValue(TenantClaimConstants.Tenant);

        public static string? GetCompany(this ClaimsPrincipal principal)
            => principal.FindFirstValue(TenantClaimConstants.Company);

        public static string? GetFullName(this ClaimsPrincipal principal)
        {
            if (principal.FindFirst(ClaimTypes.Role)?.Value == DefaultRoleConstant.Admin)
            {
                return principal?.FindFirst(ClaimTypes.Name)?.Value;
            }

            return principal?.FindFirst(TenantClaimConstants.Fullname)?.Value;

        }

        public static string? Role(this ClaimsPrincipal principal)
        {
            return principal?.FindFirst(ClaimTypes.Role)?.Value;
        }
        public static string? GetFirstName(this ClaimsPrincipal principal)
            => principal?.FindFirst(ClaimTypes.Name)?.Value;

        public static string? GetSurname(this ClaimsPrincipal principal)
            => principal?.FindFirst(ClaimTypes.Surname)?.Value;

        public static string? GetPhoneNumber(this ClaimsPrincipal principal)
            => principal.FindFirstValue(ClaimTypes.MobilePhone);

        public static string? GetUserId(this ClaimsPrincipal principal)
           => principal.FindFirstValue(ClaimTypes.NameIdentifier);

        public static string? GetImageUrl(this ClaimsPrincipal principal)
           => principal.FindFirstValue(TenantClaimConstants.ImageUrl);

        public static string? GetPermissions(this ClaimsPrincipal principal)
          => principal.FindFirstValue(TenantClaimConstants.Permission);

        public static DateTimeOffset GetExpiration(this ClaimsPrincipal principal) =>
            DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(
                principal.FindFirstValue(TenantClaimConstants.Expiration)));

        private static string? FindFirstValue(this ClaimsPrincipal principal, string claimType) =>
            principal is null
                ? throw new ArgumentNullException(nameof(principal))
                : principal.FindFirst(claimType)?.Value;
    }
}