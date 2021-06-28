using System.Threading;
using System.Threading.Tasks;
using PoC.Example.Abstract.Capabilities.CustomerInformationMgmt;

namespace PoC.Example.Example
{
    public class ExampleController
    {
        private readonly ICustomerInformationMgmtCapability _customerInformationMgmt;

        public ExampleController(ICustomerInformationMgmtCapability customerInformationMgmt)
        {
            _customerInformationMgmt = customerInformationMgmt;
        }

        public Task<string> CreatePersonAsync(Person person, CancellationToken cancellationToken)
        {
            return _customerInformationMgmt.Person.CreateAsync(person,cancellationToken);
        }
    }
}