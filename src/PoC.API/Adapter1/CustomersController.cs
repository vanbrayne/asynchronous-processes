using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nexus.Link.Libraries.Crud.Interfaces;
using Nexus.Link.Libraries.Web.AspNet.Pipe.RespondAsync;
using PoC.Example.Abstract.Capabilities.Common;
using PoC.Example.Abstract.Capabilities.CustomerInformationMgmt;
using PoC.Example.Abstract.Capabilities.CustomerOnboardingMgmt;

namespace PoC.API.Adapter1
{
    [ApiController]
    [Route("Customer")]
    public class CustomersController : ControllerBase, ICreateAndReturn<Person, string>
    {
        private readonly ICustomerOnboardingMgmt _capability;

        public CustomersController(ICustomerOnboardingMgmt capability)
        {
            _capability = capability;
        }

        /// <inheritdoc />
        [HttpPost("")]
        [RespondAsync(RespondAsyncOpinionEnum.Always)]
        public Task<Person> CreateAndReturnAsync(Person item, CancellationToken cancellationToken = default)
        {
            return _capability.Customer.CreateAndReturnAsync(item, cancellationToken);
        }
    }
}
