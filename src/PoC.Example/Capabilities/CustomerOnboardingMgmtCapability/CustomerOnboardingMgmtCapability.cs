using PoC.Example.Abstract.Capabilities.CommunicationMgmt;
using PoC.Example.Abstract.Capabilities.CustomerInformationMgmt;
using PoC.Example.Abstract.Capabilities.CustomerOnboardingMgmt;

namespace PoC.Example.Capabilities.CustomerOnboardingMgmtCapability
{
    public class CustomerOnboardingMgmtCapability : ICustomerOnboardingMgmt
    {
        public CustomerOnboardingMgmtCapability(ICustomerInformationMgmtCapability infoCapability, ICommunicationMgmtCapability comCapability)
        {
             Customer = new CustomerService(infoCapability, comCapability);
        }

        /// <inheritdoc />
        public ICustomerService Customer { get; }
    }
}
