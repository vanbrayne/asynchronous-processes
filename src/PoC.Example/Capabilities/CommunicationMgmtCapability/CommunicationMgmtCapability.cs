using PoC.Example.Abstract.Capabilities.CommunicationMgmt;

namespace PoC.Example.Capabilities.CommunicationMgmtCapability
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
