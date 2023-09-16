namespace Core.Common.Settings
{
    public class EmailSettings
    {
        public string? From { get; set; }

        public string? Host { get; set; }

        public int Port { get; set; }

        public string? UserName { get; set; }

        public string? Password { get; set; }

        public string? DisplayName { get; set; }

        public bool EnableVerification { get; set; }
        public string? VerificationBaseUrl { get; set; }
    }

    public class QuizSettings
    {
        public string QuizUrl { get; set; }
    }

}