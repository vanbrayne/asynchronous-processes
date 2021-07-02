using System.Threading;
using System.Threading.Tasks;
using PoC.Example.Abstract.Capabilities.Common;
using PoC.Example.Abstract.Capabilities.CommunicationMgmt;
using PoC.Example.Abstract.Capabilities.CustomerInformationMgmt;
using PoC.Example.Abstract.Capabilities.CustomerOnboardingMgmt;
using PoC.Example.Capabilities.CustomerInformationMgmt;

namespace PoC.Example.Capabilities.CustomerOnboardingMgmtCapability
{
    public class CustomerService : ICustomerService
    {
        private readonly ICreatePersonProcess _createPersonProcess;

        public CustomerService(ICustomerInformationMgmtCapability infoCapability, ICommunicationMgmtCapability comCapability)
        {
            _createPersonProcess = new CreatePersonProcess(infoCapability, comCapability);
        }
        /// <inheritdoc />
        public async Task<Person> CreateAndReturnAsync(Person item, CancellationToken cancellationToken = new CancellationToken())
        {
            // TODO: Always use latest version
            var person = await _createPersonProcess.ExecuteAsync($"{item.EmailAddress}", cancellationToken, item);
            return person;
        }
    }
}