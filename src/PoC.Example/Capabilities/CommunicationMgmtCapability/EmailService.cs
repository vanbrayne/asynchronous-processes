using System.Threading;
using System.Threading.Tasks;
using PoC.Example.Abstract.Capabilities.CommunicationMgmtCapability;

namespace PoC.Example.Capabilities.CommunicationMgmtCapability
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