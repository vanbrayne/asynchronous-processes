using PoC.Example.Abstract.Capabilities.CommunicationMgmtCapability;

namespace PoC.API.RestClients.CommunicationMgmtCapability
{
    public class CommunicationMgmtCapability : ICommunicationMgmtCapability
    {
        public CommunicationMgmtCapability()
        {
             Email = new EmailService();
        }

        /// <inheritdoc />
        public IEmailService Email { get; }
    }
}
