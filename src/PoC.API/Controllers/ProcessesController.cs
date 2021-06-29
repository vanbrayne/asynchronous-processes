using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using PoC.Example.Abstract.Capabilities.CustomerInformationMgmt;

namespace PoC.API.Controllers
{
    [ApiController]
    [Route("Processes")]
    public class ProcessesController : ControllerBase
    {
        private readonly ICreatePersonProcess _createPersonProcess;

        public ProcessesController(ICreatePersonProcess createPersonProcess)
        {
            _createPersonProcess = createPersonProcess;
        }

        [HttpPost("CreatePerson")]
        public Task<Person> CreatePersonAsync(Person person, CancellationToken cancellationToken = default)
        {
            return _createPersonProcess.ExecuteAsync($"CreatePerson {person.EmailAddress}", cancellationToken, person);
        }
    }
}
