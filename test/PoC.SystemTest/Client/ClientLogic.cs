using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PoC.AM.Abstract;
using PoC.AM.Abstract.Model;
using PoC.LinkLibraries;
using PoC.SystemTest.WorkFlowServer;

namespace PoC.SystemTest.Client
{

    public interface IClient
    {
        Customer Customer { get; }
        Exception Exception { get; }
        Task<Guid> OnBoardCustomerAsync(string emailAddress);
        Task<Customer> OnBoardCustomerSynchronouslyAsync(string emailAddress);
    }

    public class ClientLogic : IClient
    {
        private readonly IAsyncManagement _asyncManager;
        private readonly IWorkFlow _workFlow;
        public Customer Customer { get; private set; }

        /// <inheritdoc />
        public Exception Exception { get; private set; }

        public ClientLogic(IAsyncManagement asyncManager, IWorkFlow workFlow)
        {
            _asyncManager = asyncManager;
            _workFlow = workFlow;
        }

        /// <inheritdoc />
        public Task<Guid> OnBoardCustomerAsync(string emailAddress)
        {
            return _asyncManager.CreateAsyncRequestAsync(
                $"On board customer {emailAddress}",
                async headers => (await _workFlow.OnBoardCustomerAsync(emailAddress, headers)).ToJsonString(),
                CallBackAsync);
        }

        /// <inheritdoc />
        public Task<Customer> OnBoardCustomerSynchronouslyAsync(string emailAddress)
        {
            var headers = new Headers(null);
            return _workFlow.OnBoardCustomerAsync(emailAddress, headers);
        }

        private Task CallBackAsync(Guid requestId, string responseAsObject, Exception exception = null)
        {
            if (exception == null)
            {
                Customer = JsonConvert.DeserializeObject<Customer>(responseAsObject);
            }
            else
            {
                Exception = exception;
            }

            return Task.CompletedTask;
        }
    }
}