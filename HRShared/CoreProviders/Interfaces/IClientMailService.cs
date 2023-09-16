using HRShared.Common;

namespace HRShared.CoreProviders.Interfaces
{
    public interface IClientMailService
    {
        Task SendAsync(ClientMailRequest request);
        Task SendListAsync(List<ClientMailRequest> requests);
    }
}