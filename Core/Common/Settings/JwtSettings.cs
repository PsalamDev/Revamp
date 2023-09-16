namespace Core.Common.Settings
{
    public class JwtSettings
    {
        public string? Key { get; set; }

        public string? ValidAudience { get; set; }

        public string? ValidIssuer { get; set; }

        public int TokenExpirationInMinutes { get; set; }

        public int RefreshTokenExpirationInDays { get; set; }
    }
}