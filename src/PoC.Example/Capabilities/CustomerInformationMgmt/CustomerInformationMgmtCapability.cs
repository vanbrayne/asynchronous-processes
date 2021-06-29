using PoC.Example.Abstract.Capabilities.CommunicationMgmt;
using PoC.Example.Abstract.Capabilities.CustomerInformationMgmt;
using PoC.Example.Persistence;

namespace PoC.Example.Capabilities.CustomerInformationMgmt
{
    public class CustomerInformationMgmtCapability : ICustomerInformationMgmtCapability
    {
        public CustomerInformationMgmtCapability(ICommunicationMgmtCapability communicationMgmtCapability)
        {
            Person = new PersonService(this, new PersonTable(), communicationMgmtCapability);
        }

        /// <inheritdoc />
        public IPersonService Person { get; }
    }
}
