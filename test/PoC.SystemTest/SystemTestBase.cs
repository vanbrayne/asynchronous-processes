using System;
using System.Threading.Tasks;
using PoC.AM;
using PoC.AM.Abstract;
using PoC.SystemTest.BusinessApiServer;
using PoC.SystemTest.Client;
using PoC.SystemTest.WorkFlowServer;
using Xunit;
using Xunit.Abstractions;

namespace PoC.SystemTest
{
    public class SystemTestBase : IClassFixture<SystemTestFixture>
    {
        protected readonly SystemTestFixture Fixture;
        protected readonly ITestOutputHelper Output;

        public SystemTestBase(SystemTestFixture fixture, ITestOutputHelper output)
        {
            fixture.AsyncManager.Clear();
            fixture.WorkFlow.InPipe.Queue.Clear();
            fixture.BusinessApi.InPipe.Queue.Clear();
            Fixture = fixture;
            Output = output;
        }

        protected async Task CompleteExecutionAsync(Guid requestId)
        {
            await WriteRequestStatus(requestId);

            while (await Fixture.AsyncManager.CallFirstInQueue())
            {
                // await WriteRequestStatus(requestId);
                while (await Fixture.WorkFlow.InPipe.ExecuteFromQueueAsync())
                {
                    await WriteRequestStatus(requestId);
                }

                while (await Fixture.BusinessApi.InPipe.ExecuteFromQueueAsync())
                {
                    await WriteRequestStatus(requestId);
                }
            }
        }

        protected async Task WriteRequestStatus(Guid requestId)
        {
            Output.WriteLine(await Fixture.AsyncManager.GetStatusAsStringAsync(requestId));
        }
    }
    public class SystemTestFixture
    {
        public IAsyncManagement AsyncManager { get; }
        public IWorkFlow WorkFlow { get; }
        public IBusinessApi BusinessApi { get; }
        public IClient Client { get; }

        public SystemTestFixture()
        {
            AsyncManager = new AsyncManager();
            BusinessApi = new BusinessApi(AsyncManager);
            WorkFlow = new WorkFlowLogic(AsyncManager, BusinessApi);
            Client = new ClientLogic(AsyncManager, WorkFlow);
        }
    }
}
