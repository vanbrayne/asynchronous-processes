using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nexus.Link.Libraries.Core.Assert;
using Nexus.Link.Libraries.Crud.Interfaces;
using Nexus.Link.Libraries.Web.AspNet.Pipe.RespondAsync;
using PoC.Example.Abstract.Capabilities.Common;
using PoC.Example.Abstract.Capabilities.CustomerInformationMgmt;
using PoC.Example.Abstract.Capabilities.CustomerOnboardingMgmt;

namespace PoC.API.Adapter1
{
    [ApiController]
    [Route("Customers")]
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
        public async Task<Person> CreateAndReturnAsync(Person person, CancellationToken cancellationToken = default)
        {
            var result = await _capability.Customer.CreateAndReturnAsync(person, cancellationToken);
            return result;
        }

        [HttpGet("Tests/{number}")]
        [RespondAsync(RespondAsyncOpinionEnum.Always)]
        public async Task<Person> PreparedTest(int number, CancellationToken cancellationToken = default)
        {
            ServiceContract.RequireGreaterThanOrEqualTo(1, number, nameof(number));
            ServiceContract.RequireLessThanOrEqualTo(1, number, nameof(number));
            switch (number)
            {
                case 1:
                    {
                        var person = new Person
                        {
                            EmailAddress = "jane.doe@example.com",
                            PersonalNumber = "990529-0707"
                        };
                        var result = await _capability.Customer.CreateAndReturnAsync(person, cancellationToken);
                        return result;
                    }
                default:
                    FulcrumAssert.Fail($"Unexpected test number: {number}");
                    throw new ArgumentException();
            }
        }
    }
}
