using HRShared.Common;

namespace HRShared.CoreProviders.Interfaces
{
    public interface ISMS
    {
        Task<bool> SendAsync(SMSRequest request);
    }
}