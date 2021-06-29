using Nexus.Link.Libraries.Web.RestClientHelper;
using PoC.Example.Abstract.Capabilities.CommunicationMgmt;
using PoC.Example.Abstract.Capabilities.CustomerInformationMgmt;
using PoC.Example.Capabilities.CustomerInformationMgmt;

namespace PoC.API.RestClients.CustomerInformationMgmt
{
    public class CustomerInformationMgmtRestClient : ICustomerInformationMgmtCapability
    {
        public CustomerInformationMgmtRestClient(IHttpSender httpSender, ICommunicationMgmtCapability communicationMgmtCapability)
        {
            CreatePersonProcess = new CreatePersonProcess(this, communicationMgmtCapability);
            Person = new PersonRestClient(httpSender.CreateHttpSender("Persons"));
        }

        /// <inheritdoc />
        public IPersonService Person { get; }

        /// <inheritdoc />
        public ICreatePersonProcess CreatePersonProcess { get; }
    }
}
