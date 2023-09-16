namespace Infrastructure.Providers.Settings
{
    public record AzureStorageSettings
    {
        public string? StorageConnectionString { get; set; }
    }
}