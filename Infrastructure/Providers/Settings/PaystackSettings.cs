namespace Infrastructure.Providers.Settings
{
    public class PaystackSettings
    {
        public string PublicKey { get; set; } = default!;
        public string SecretKey { get; set; } = default!;
        public string BaseUrl { get; set; } = default!;
    }
}