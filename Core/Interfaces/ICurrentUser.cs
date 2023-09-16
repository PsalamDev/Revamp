using System.Security.Claims;

namespace Core.Interfaces
{
    public interface ICurrentUserInitializer
    {
        void SetCurrentUser(ClaimsPrincipal user);

        void SetCurrentUserId(string userId);
    }


    public interface ICurrentUser
    {
        string? Name { get; }

        Guid GetUserId();

        string? GetUserEmail();
        string? GetFullname();
        string? GetImageUrl();
        string? GetCompany();

        string? GetTenant();

        bool IsAuthenticated();

        bool IsInRole(string role);
        string? Role();
        IEnumerable<Claim>? GetUserClaims();
    }
}