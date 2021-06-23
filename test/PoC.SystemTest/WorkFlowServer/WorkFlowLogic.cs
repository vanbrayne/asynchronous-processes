using System;
using System.Threading.Tasks;
using PoC.AM.Abstract;
using PoC.AM.Abstract.Exceptions;
using PoC.AM.Abstract.Model;
using PoC.LinkLibraries;
using PoC.SystemTest.BusinessApiServer;
using ExecutionContext = PoC.AM.Abstract.Model.ExecutionContext;
using Person = PoC.SystemTest.WorkFlowServer.Experiment.Person;

namespace PoC.SystemTest.WorkFlowServer
{

    public interface IWorkFlow
    {
        InPipe InPipe { get; }
        Task<Customer> OnBoardCustomerAsync(string emailAddress, Headers headers);
    }
    public class WorkFlowLogic : IWorkFlow
    {
        private readonly IBusinessApi _businessApi;
        private readonly RestClient _restClient;
        public InPipe InPipe { get; }
        private static WorkFlowPersistence Persistence { get; } = new WorkFlowPersistence();

        public WorkFlowLogic(IAsyncManagement asyncManager, IBusinessApi businessApi)
        {
            _businessApi = businessApi;
            InPipe = new InPipe(asyncManager);
            var outPipe = new OutPipe(asyncManager);
            _restClient = new RestClient(outPipe);
        }

        /// <inheritdoc />
        public async Task<Customer> OnBoardCustomerAsync(string emailAddress, Headers headers)

        {
            var context = await InPipe.PreInvokeAsync(headers);
            try
            {
                switch (context.Version)
                {
                    case 0:
                        return await OnBoardCustomerAsyncLogicVersion0(emailAddress, context);
                    default:
                        throw new ProgrammersErrorException($"Unknown version: {context.Version}");
                }
            }
            finally
            {
                await InPipe.PostInvokeAsync(headers, context);
            }
        }

        private async Task<Customer> OnBoardCustomerAsyncLogicVersion0(string emailAddress, ExecutionContext context)

        {
            if (string.IsNullOrWhiteSpace(emailAddress)) throw new ArgumentException("Email address was empty.");

            // Asynchronous call
            AsyncHelper.SetNextRequest(context, "Get personal info");
            var person = await _restClient.SendRequestAsync<Person>(
                async h => (await _businessApi.GetPersonalInfoAsync(emailAddress, h)).ToJsonString(), context);

            var customer = new Customer
            {
                EmailAddress = emailAddress,
                Name = person.Name
            };

            // Idempotent synchronous call
            AsyncHelper.SetNextRequest(context, "Create customer");
            var customerId = await _restClient.MakeCallIdempotent(context,
                () => Persistence.CreateCustomerAsync(customer));

            // Idempotent synchronous call
            AsyncHelper.SetNextRequest(context, "Get customer");
            customer = await _restClient.MakeCallIdempotent(context,
                () => Persistence.GetCustomerAsync(customerId));

            // Two asynchronous calls in parallel
            AsyncHelper.SetNextRequest(context, "Create credit account");
            var creditAccountTask =
                _restClient.SendRequestAsync<Guid>(
                    async h => (await _businessApi.CreateAccount("Credit", h)).ToJsonString(), context);
            AsyncHelper.SetNextRequest(context, "Create savings account");
            var savingsAccountTask =
                _restClient.SendRequestAsync<Guid>(
                    async h => (await _businessApi.CreateAccount("Savings", h)).ToJsonString(), context);
            customer.CreditAccountId = await creditAccountTask;
            customer.SavingsAccountId = await savingsAccountTask;

            // Idempotent synchronous call
            AsyncHelper.SetNextRequest(context, "Update customer");
            await _restClient.MakeCallIdempotent(context,
                () => Persistence.UpdateCustomerAsync(customerId, customer));

            // Idempotent synchronous call
            AsyncHelper.SetNextRequest(context, "Get customer");
            customer = await _restClient.MakeCallIdempotent(context,
                () => Persistence.GetCustomerAsync(customerId));

            // Asynchronous call
            AsyncHelper.SetNextRequest(context, "Email welcome message");
            await _restClient.SendRequestAsync<string>(
                async h =>
                {
                    await _businessApi.SendEmail(customer.EmailAddress, "Welcome",
                        $"Welcome {customer.Name} you are now a customer to Async Inc.", h);
                    return null;
                }, context);

            return await Task.FromResult(customer);
        }
    }
}