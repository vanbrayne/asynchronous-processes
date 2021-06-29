using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using PoC.Example.Abstract.Capabilities.CommunicationMgmt;

namespace PoC.API.Controllers
{
    [ApiController]
    [Route("Persons")]
    public class EmailsController : ControllerBase, IEmailService
    {
        private readonly ICommunicationMgmtCapability _capability;

        public EmailsController(ICommunicationMgmtCapability capability)
        {
            _capability = capability;
        }

        /// <inheritdoc />
        [HttpPost("")]
        public Task SendEmailAsync(Email email, CancellationToken cancellationToken = default)
        {
            return _capability.Email.SendEmailAsync(email, cancellationToken);
        }
    }
}
