using HRShared.Common;
using HRShared.CoreProviders.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Asn1.X509;

namespace HRShared.CoreProviders.Implementation
{
    public class SMSService : ISMS
    {
        private readonly ILogger<SMSService> _logger;
        private readonly IConfiguration _configuration;
        private HttpClient _httpClient;

        public SMSService(ILogger<SMSService> logger, IConfiguration configuration, HttpClient httpClient)
        {
            _logger = logger;
            _configuration = configuration;
            _httpClient = httpClient;
        }

        public async Task<bool> SendAsync(SMSRequest request)
        {
            bool isSent = false;

            try
            {
                var userName = _configuration.GetSection("SMS:Username").Value;
                var password = _configuration.GetSection("SMS:Password").Value;
                var sender = _configuration.GetSection("SMS:Sender").Value;
                string APIURL = $"?username={userName}&password={password}&sender={sender}&recipient={request.PhoneNumber}&message={request.Message}";
                var response = await _httpClient.GetAsync(APIURL);

                if (response.IsSuccessStatusCode)
                    isSent = true;

                var message = await response.Content.ReadAsStringAsync();

                return isSent;
            }
            catch (Exception ex)
            {
                _logger.LogError("An Error Occured :" + ex.StackTrace);
                return isSent;
            }

        }
    }
}