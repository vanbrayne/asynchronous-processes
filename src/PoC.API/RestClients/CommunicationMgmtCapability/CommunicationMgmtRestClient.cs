using Nexus.Link.Libraries.Web.RestClientHelper;
using PoC.Example.Abstract.Capabilities.CommunicationMgmt;

namespace PoC.API.RestClients.CommunicationMgmtCapability
{
    public class CommunicationMgmtRestClient: ICommunicationMgmtCapability
    {
        public CommunicationMgmtRestClient(IHttpSender httpSender)
        {
             Email = new EmailRestClient(httpSender.CreateHttpSender("Emails"));
        }

        /// <inheritdoc />
        public IEmailService Email { get; }
    }
}
