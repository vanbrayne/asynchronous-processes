using System.Threading;
using System.Threading.Tasks;
using Nexus.Link.Libraries.Crud.Interfaces;

namespace PoC.SystemTest.WorkFlowServer.Experiment.Capabilities.CommunicationMgmtCapability
{
    public interface IEmailService
    {
        Task SendEmailAsync(string emailAddress, string subject, string message, CancellationToken cancellationToken = default);
    }
}