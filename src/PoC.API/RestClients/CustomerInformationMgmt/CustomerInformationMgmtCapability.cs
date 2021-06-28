using Nexus.Link.Libraries.Web.RestClientHelper;
using PoC.Example.Abstract.Capabilities.CommunicationMgmtCapability;
using PoC.Example.Abstract.Capabilities.CustomerInformationMgmt;

namespace PoC.API.RestClients.CustomerInformationMgmt
{
    public class CustomerInformationMgmtCapability : ICustomerInformationMgmtCapability
    {
        public CustomerInformationMgmtCapability(ICommunicationMgmtCapability communicationMgmtCapability)
        {
            Person = new PersonService(new HttpSender("http://localhost:6310/Persons"));
        }

        /// <inheritdoc />
        public IPersonService Person { get; }
    }
}
