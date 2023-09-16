using Core.Common.Model;

namespace Infrastructure.Providers.Interface
{
    public interface IMailService
    {
        Task SendAsync(MailRequest request);
        Task SendListAsync(List<MailRequest> requests);
    }
}
