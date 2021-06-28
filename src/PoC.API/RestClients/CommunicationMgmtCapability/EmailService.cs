using System.Threading;
using System.Threading.Tasks;
using PoC.Example.Abstract.Capabilities.CommunicationMgmtCapability;

namespace PoC.API.RestClients.CommunicationMgmtCapability
{
    public class EmailService : IEmailService
    {
        /// <inheritdoc />
        public async Task SendEmailAsync(string emailAddress, string subject, string message, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }
    }
}