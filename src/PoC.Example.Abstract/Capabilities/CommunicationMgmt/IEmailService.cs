using System.Threading;
using System.Threading.Tasks;

namespace PoC.Example.Abstract.Capabilities.CommunicationMgmt
{
    public interface IEmailService
    {
        Task SendEmailAsync(Email email, CancellationToken cancellationToken = default);
    }

    public class Email
    {
        public Email(string to, string subject, string message = null)
        {
            To = to;
            Subject = subject;
            Message = message;
        }
        public string To { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
    }
}