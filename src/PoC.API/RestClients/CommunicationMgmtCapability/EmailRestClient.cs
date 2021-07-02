using System.Threading;
using System.Threading.Tasks;
using Nexus.Link.Libraries.Web.RestClientHelper;
using PoC.Example.Abstract.Capabilities.CommunicationMgmt;

namespace PoC.API.RestClients.CommunicationMgmtCapability
{
    public class EmailRestClient : RestClient, IEmailService
    {
        /// <inheritdoc />
        public EmailRestClient(IHttpSender httpSender) : base(httpSender)
        {
        }

        /// <inheritdoc />
        public Task SendEmailAsync(Email email, CancellationToken cancellationToken = default)
        {
            return PostNoResponseContentAsync("", email, null, cancellationToken);
        }
    }
}