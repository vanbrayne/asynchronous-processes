using PoC.Example.Abstract.Capabilities.CommunicationMgmt;
using PoC.Example.Abstract.Capabilities.CustomerInformationMgmt;
using PoC.Example.Persistence;

namespace PoC.Example.Capabilities.CustomerInformationMgmt
{
    public class CustomerInformationMgmtCapability : ICustomerInformationMgmtCapability
    {
        public CustomerInformationMgmtCapability(ICommunicationMgmtCapability communicationMgmtCapability, ICreatePersonProcess createPersonProcess)
        {
            CreatePersonProcess = createPersonProcess;
            Person = new PersonService(this, new PersonTable());
        }

        /// <inheritdoc />
        public IPersonService Person { get; }

        /// <inheritdoc />
        public ICreatePersonProcess CreatePersonProcess { get; }
    }
}
