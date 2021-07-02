using System.Threading;
using System.Threading.Tasks;
using PoC.Example.Abstract.Capabilities.CommunicationMgmt;

namespace PoC.Example.Capabilities.CommunicationMgmtCapability
{
    public class EmailService : IEmailService
    {
           /// <inheritdoc />
        public Task SendEmailAsync(Email email, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
            //throw new System.NotImplementedException();
        }
    }
}