using PoC.Example.Abstract.Capabilities.CommunicationMgmtCapability;
using PoC.Example.Abstract.Capabilities.CustomerInformationMgmt;
using PoC.Example.Capabilities.CustomerInformationMgmt.CreatePersonProcess;

namespace PoC.Example.Capabilities.CustomerInformationMgmt
{
    public class CustomerInformationMgmtCapability : ICustomerInformationMgmtCapability
    {
        public CustomerInformationMgmtCapability(ICommunicationMgmtCapability communicationMgmtCapability)
        {
            Person = new PersonService(new CreatePersonProcessDefinition(this, communicationMgmtCapability));
        }

        /// <inheritdoc />
        public IPersonService Person { get; }
    }
}
