using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using PoC.Example.Abstract.Capabilities.CustomerInformationMgmt;

namespace PoC.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PersonsController : ControllerBase, IPersonService
    {
        private readonly ICustomerInformationMgmtCapability _capability;

        public PersonsController(ICustomerInformationMgmtCapability capability)
        {
            _capability = capability;
        }

        /// <inheritdoc />
        [HttpGet("{id}")]
        public Task<Person> ReadAsync(string id, CancellationToken cancellationToken = default)
        {
            return _capability.Person.ReadAsync(id, cancellationToken);
        }

        /// <inheritdoc />
        [HttpPost("")]
        public Task<string> CreateAsync(Person item, CancellationToken cancellationToken = default)
        {
            return _capability.Person.CreateAsync(item, cancellationToken);
        }

        /// <inheritdoc />
        [HttpGet("?personalNumber={personalNumber}")]
        public Task<Person> GetByPersonalNumberAsync(string personalNumber, CancellationToken cancellationToken = default)
        {
            return _capability.Person.GetByPersonalNumberAsync(personalNumber, cancellationToken);
        }

        /// <inheritdoc />
        public Task<Person> GetOfficialInformationAsync(Person person, CancellationToken cancellationToken = default)
        {
            return _capability.Person.GetOfficialInformationAsync(person, cancellationToken);
        }

        /// <inheritdoc />
        [HttpPost("{id}/AskForDetails")]
        public Task<Person> AskUserToFillInDetailsAsync(string id, Person person, CancellationToken cancellationToken)
        {
            return _capability.Person.AskUserToFillInDetailsAsync(id, person, cancellationToken);
        }

        /// <inheritdoc />
        public  Task<bool> ValidateAsync(Person person, CancellationToken cancellationToken)
        {
            return _capability.Person.ValidateAsync(person, cancellationToken);
        }
    }
}
