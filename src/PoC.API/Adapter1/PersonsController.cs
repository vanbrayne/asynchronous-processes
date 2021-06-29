using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using Nexus.Link.Libraries.Web.AspNet.Pipe.RespondAsync;
using PoC.Example.Abstract.Capabilities.CustomerInformationMgmt;

namespace PoC.API.Controllers
{
    [ApiController]
    [Route("Persons")]
    public class PersonsController : ControllerBase, IPersonService
    {
        private readonly ICustomerInformationMgmtCapability _capability;

        public PersonsController(ICustomerInformationMgmtCapability capability)
        {
            _capability = capability;
        }

        /// <inheritdoc />
        [HttpGet("")]
        public Task<IEnumerable<Person>> ReadAllAsync(int limit = int.MaxValue, CancellationToken cancellationToken = default)
        {
            return _capability.Person.ReadAllAsync(limit, cancellationToken);
        }

        /// <inheritdoc />
        [HttpGet("{id}")]
        public Task<Person> ReadAsync(string id, CancellationToken cancellationToken = default)
        {
            return _capability.Person.ReadAsync(id, cancellationToken);
        }

        /// <inheritdoc />
        [HttpPost("")]
        [RespondAsync(RespondAsyncOpinionEnum.Always)]
        public Task<Person> CreateAndReturnAsync(Person item, CancellationToken cancellationToken = default)
        {
            return _capability.Person.CreateAndReturnAsync(item, cancellationToken);
        }

        /// <inheritdoc />
        [HttpGet("/By")]
        public Task<Person> GetByPersonalNumberAsync([FromQuery(Name = "personalNumber")] string personalNumber, CancellationToken cancellationToken = default)
        {
            return _capability.Person.GetByPersonalNumberAsync(personalNumber, cancellationToken);
        }

        /// <inheritdoc />
        [HttpPost("OfficialInformation")]
        public Task<Person> GetOfficialInformationAsync(Person person, CancellationToken cancellationToken = default)
        {
            return _capability.Person.GetOfficialInformationAsync(person, cancellationToken);
        }

        /// <inheritdoc />
        [HttpPost("AskForDetails")]
        public Task<Person> AskUserToFillInDetailsAsync(Person person, CancellationToken cancellationToken)
        {
            return _capability.Person.AskUserToFillInDetailsAsync(person, cancellationToken);
        }

        /// <inheritdoc />
        [HttpPost("Validate")]
        public Task<bool> ValidateAsync(Person person, CancellationToken cancellationToken)
        {
            return _capability.Person.ValidateAsync(person, cancellationToken);
        }
    }
}
