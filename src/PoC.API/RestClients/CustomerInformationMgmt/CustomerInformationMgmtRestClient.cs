using Nexus.Link.Libraries.Web.RestClientHelper;
using PoC.Example.Abstract.Capabilities.CommunicationMgmt;
using PoC.Example.Abstract.Capabilities.CustomerInformationMgmt;
using PoC.Example.Abstract.Capabilities.CustomerOnboardingMgmt;
using PoC.Example.Capabilities.CustomerInformationMgmt;
using PoC.Example.Capabilities.CustomerOnboardingMgmtCapability;
using IPersonService = PoC.Example.Abstract.Capabilities.CustomerInformationMgmt.IPersonService;

namespace PoC.API.RestClients.CustomerInformationMgmt
{
    public class CustomerInformationMgmtRestClient : ICustomerInformationMgmtCapability
    {
        public CustomerInformationMgmtRestClient(IHttpSender httpSender)
        {
            Person = new PersonRestClient(httpSender.CreateHttpSender("Persons"));
        }

        /// <inheritdoc />
        public IPersonService Person { get; }
    }
}
