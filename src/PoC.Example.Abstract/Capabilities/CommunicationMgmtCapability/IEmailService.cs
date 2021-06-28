using System.Threading;
using System.Threading.Tasks;

namespace PoC.Example.Abstract.Capabilities.CommunicationMgmtCapability
{
    public interface IEmailService
    {
        Task SendEmailAsync(string emailAddress, string subject, string message, CancellationToken cancellationToken = default);
    }
}