using Microsoft.Extensions.Hosting;
using System.Text;

namespace Infrastructure.Helpers
{
    public static class MailTemplateHelper
    {
        private static IHostEnvironment _hostingEnvironment;

        public static bool IsInitialized { get; private set; }

        public static void Initialize(IHostEnvironment hostEnvironment)
        {
            if (IsInitialized)
                throw new InvalidOperationException("Object already initialized");

            _hostingEnvironment = hostEnvironment;
            IsInitialized = true;
        }

        public static string GenerateMailContent(string htmlContent, string htmlPath, string organisationName)
        {
            if (htmlContent == null || htmlPath == null)
            {
                return null;
            }
            string baseDirectory = _hostingEnvironment.ContentRootPath;
            string tmplFolder = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplates");
            string filePath = Path.Combine(tmplFolder, htmlPath);

            using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var sr = new StreamReader(fs, Encoding.Default);
            string mailText = sr.ReadToEnd();
            sr.Close();

            if (string.IsNullOrEmpty(mailText))
            {
                return htmlContent;
            }

            return mailText.Replace("[[EMAILCONTENT]]", htmlContent).Replace("[[ORGANIZATION]]", organisationName);
        }


        public static string GenerateEmailConfirmationEmail(string userName, string email, string emailVerificationUri, string emailVerificationUriText, string? html = null, string? plainText = null)
        {
            var htmlString = plainText ?? string.Format("Click on the link to complete action <a href='{0}'>clicking here</a>.", emailVerificationUri);

            if (html == null)
            {
                return htmlString;
            }
            string baseDirectory = _hostingEnvironment.ContentRootPath;
            string tmplFolder = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplates");
            string filePath = Path.Combine(tmplFolder, html);

            using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var sr = new StreamReader(fs, Encoding.Default);
            string mailText = sr.ReadToEnd();
            sr.Close();

            if (string.IsNullOrEmpty(mailText))
            {
                return htmlString;
            }

            return mailText.Replace("[userName]", userName).Replace("[email]", email).Replace("[emailVerificationUri]", emailVerificationUri).Replace("[emailVerificationUriText]", emailVerificationUriText);
        }
    }
}