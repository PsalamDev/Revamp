namespace HRShared.Common
{
    public class JwtSetting
    {
        public string? Key { get; set; }
       


        public int TokenExpirationInMinutes { get; set; }

        public int RefreshTokenExpirationInDays { get; set; }
    }
}