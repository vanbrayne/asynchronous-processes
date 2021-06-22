using System;
using System.Threading.Tasks;
using PoC.AM.Abstract;
using PoC.AM.Abstract.Model;
using PoC.LinkLibraries;

namespace PoC.SystemTest.BusinessApiServer
{

    public interface IBusinessApi
    {
        InPipe InPipe { get; }
        Task<Person> GetPersonalInfoAsync(string emailAddress, Headers headers);
        Task<Guid> CreateAccount(string name, Headers headers);
        Task SendEmail(string emailAddress, string subject, string message, Headers headers);

        Task<string> GetPublicAddress(string emailAddress, Headers headers);
    }

    public class BusinessApi : IBusinessApi
    {
        private readonly RestClient _restClient;

        public InPipe InPipe { get; }
        public static string FailForThisEmailAddress { get; } = "fail@example.com";

        public BusinessApi(IAsyncManagement asyncManager)
        {
            InPipe = new InPipe(asyncManager);
            var outPipe = new OutPipe(asyncManager);
            _restClient = new RestClient(outPipe);
        }

        /// <inheritdoc />
        public async Task<Person> GetPersonalInfoAsync(string emailAddress, Headers headers)
        {
            var context = await InPipe.PreInvokeAsync(headers);
            try
            {
                if (emailAddress == FailForThisEmailAddress) throw new ArgumentException($"BusinessApi always fails for email address {FailForThisEmailAddress}.");

                // Asynchronous call
                AsyncHelper.SetNextRequest(context, "Get public address");
                var address = await _restClient.SendRequestAsync<string>(
                    async h => (await GetPublicAddress(emailAddress, h)).ToJsonString(), context);

                var person = new Person { Name = "Joe", Address = address };
                return person;
            }
            finally
            {
                await InPipe.PostInvokeAsync(headers, context);
            }
        }

        /// <inheritdoc />
        public async Task<Guid> CreateAccount(string name, Headers headers)
        {
            var context = await InPipe.PreInvokeAsync(headers);
            try
            {
                var account = new Account
                {
                    Id = Guid.NewGuid(),
                    Name = name
                };
                return account.Id;
            }
            finally
            {
                await InPipe.PostInvokeAsync(headers, context);
            }
        }

        /// <inheritdoc />
        public async Task SendEmail(string emailAddress, string subject, string message, Headers headers)
        {
            var context = await InPipe.PreInvokeAsync(headers);
            try
            {
                // No implementation right now
            }
            finally
            {
                await InPipe.PostInvokeAsync(headers, context);
            }
        }

        /// <inheritdoc />
        public async Task<string> GetPublicAddress(string emailAddress, Headers headers)
        {
            var context = await InPipe.PreInvokeAsync(headers);
            try
            {
                return "Official address";
            }
            finally
            {
                await InPipe.PostInvokeAsync(headers, context);
            }
        }
    }
}