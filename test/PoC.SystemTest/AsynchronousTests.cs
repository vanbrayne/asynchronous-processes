using System;
using System.Linq;
using System.Threading.Tasks;
using PoC.AM.Abstract.Exceptions;
using PoC.SystemTest.BusinessApiServer;
using Xunit;
using Xunit.Abstractions;

namespace PoC.SystemTest
{
    public class AsynchronousTests : SystemTestBase
    {
        public AsynchronousTests(SystemTestFixture fixture, ITestOutputHelper output)
            : base(fixture, output)
        {
        }

        [Fact]
        public async Task T01AsyncManagerStoresRequest()
        {
            const string expectedEmailAddress = "john.doe@example.com";
            var expectedRequestId = await Fixture.Client.OnBoardCustomerAsync(expectedEmailAddress);
            Assert.True(Fixture.AsyncManager.Requests.ContainsKey(expectedRequestId));
        }

        [Fact]
        public async Task T02AsyncManagerEnqueueRequest()
        {
            const string expectedEmailAddress = "john.doe@example.com";
            var expectedRequestId = await Fixture.Client.OnBoardCustomerAsync(expectedEmailAddress);
            Assert.True(Fixture.AsyncManager.RequestQueue.TryPeek(out var actualRequestId));
            Assert.Equal(expectedRequestId, actualRequestId);
        }

        [Fact]
        public async Task T03WorkFlowStoresRequest()
        {
            const string expectedEmailAddress = "john.doe@example.com";
            var expectedRequestId = await Fixture.Client.OnBoardCustomerAsync(expectedEmailAddress);
            var success = await Fixture.AsyncManager.CallFirstInQueue();
            Assert.True(success);
            Assert.Collection(Fixture.WorkFlow.InPipe.Queue, i =>
            {
                Assert.True(Fixture.AsyncManager.Executions.TryGetValue(i.ExecutionId, out var execution));
                Assert.Equal(expectedRequestId, execution.RequestId);
            });
        }

        [Fact]
        public async Task T04WorkFlowMethodIsActivatedFromQueue()
        {
            const string expectedEmailAddress = "john.doe@example.com";
            await Fixture.Client.OnBoardCustomerAsync(expectedEmailAddress);
            var success = await Fixture.AsyncManager.CallFirstInQueue();
            Assert.True(success);
            success = await Fixture.WorkFlow.InPipe.ExecuteFromQueueAsync();
            Assert.True(success);
        }

        [Fact]
        public async Task T05AsyncManagerSavesExecution()
        {
            const string expectedEmailAddress = "john.doe@example.com";
            var requestId = await Fixture.Client.OnBoardCustomerAsync(expectedEmailAddress);
            var success = await Fixture.AsyncManager.CallFirstInQueue();
            Assert.True(success);
            success = await Fixture.WorkFlow.InPipe.ExecuteFromQueueAsync();
            Assert.True(success);
            var request = Fixture.AsyncManager.Requests[requestId];
            Assert.Collection(Fixture.AsyncManager.Executions.Values, execution => Assert.False(execution.HasCompleted));
        }

        [Fact]
        public async Task T06AsyncManagerStoresSubRequest()
        {
            const string expectedEmailAddress = "john.doe@example.com";
            var requestId = await Fixture.Client.OnBoardCustomerAsync(expectedEmailAddress);
            var success = await Fixture.AsyncManager.CallFirstInQueue();
            Assert.True(success);
            success = await Fixture.WorkFlow.InPipe.ExecuteFromQueueAsync();
            Assert.True(success);

            Assert.True(Fixture.AsyncManager.Requests.ContainsKey(requestId));
            var request = Fixture.AsyncManager.Requests[requestId];
            Assert.NotNull(request.LatestExecutionId);
            var subRequest = Fixture.AsyncManager.ExecutionContexts[request.LatestExecutionId.Value].SubRequests.Values.FirstOrDefault();
            Assert.NotNull(subRequest);
            Assert.NotNull(subRequest.RequestId);
            Assert.True(Fixture.AsyncManager.Requests.ContainsKey(subRequest.RequestId.Value));
        }

        [Fact]
        public async Task T07AsyncManagerEnqueueSubRequest()
        {
            const string expectedEmailAddress = "john.doe@example.com";
            var requestId = await Fixture.Client.OnBoardCustomerAsync(expectedEmailAddress);
            var success = await Fixture.AsyncManager.CallFirstInQueue();
            Assert.True(success);
            success = await Fixture.WorkFlow.InPipe.ExecuteFromQueueAsync();
            Assert.True(success);

            Assert.True(Fixture.AsyncManager.Requests.ContainsKey(requestId));
            var request = Fixture.AsyncManager.Requests[requestId];
            Assert.NotNull(request.LatestExecutionId);
            var subRequest = Fixture.AsyncManager.ExecutionContexts[request.LatestExecutionId.Value].SubRequests.Values.FirstOrDefault();
            Assert.NotNull(subRequest);
            Assert.NotNull(subRequest.RequestId);
            Assert.True(Fixture.AsyncManager.Requests.ContainsKey(subRequest.RequestId.Value));
            Assert.True(Fixture.AsyncManager.RequestQueue.TryPeek(out var actualRequestId));
            Assert.Equal(subRequest.RequestId, actualRequestId);
        }

        [Fact]
        public async Task T08BusinessApiStoresRequest()
        {
            const string expectedEmailAddress = "john.doe@example.com";
            var requestId = await Fixture.Client.OnBoardCustomerAsync(expectedEmailAddress);
            var success = await Fixture.AsyncManager.CallFirstInQueue();
            Assert.True(success);
            success = await Fixture.WorkFlow.InPipe.ExecuteFromQueueAsync();
            Assert.True(success);

            var request = Fixture.AsyncManager.Requests[requestId];
            Assert.NotNull(request.LatestExecutionId);
            var subRequest = Fixture.AsyncManager.ExecutionContexts[request.LatestExecutionId.Value].SubRequests.Values.FirstOrDefault();
            Assert.NotNull(subRequest);

            success = await Fixture.AsyncManager.CallFirstInQueue();
            Assert.True(success);
            Assert.Collection(Fixture.BusinessApi.InPipe.Queue, i =>
            {
                Assert.True(Fixture.AsyncManager.Executions.TryGetValue(i.ExecutionId, out var execution));
                Assert.Equal(subRequest.RequestId, execution.RequestId);
            });
        }

        [Fact]
        public async Task T09BusinessApiMethodIsActivatedFromQueue()
        {
            const string expectedEmailAddress = "john.doe@example.com";
            await Fixture.Client.OnBoardCustomerAsync(expectedEmailAddress);
            var success = await Fixture.AsyncManager.CallFirstInQueue();
            Assert.True(success);
            success = await Fixture.WorkFlow.InPipe.ExecuteFromQueueAsync();
            Assert.True(success);

            success = await Fixture.AsyncManager.CallFirstInQueue();
            Assert.True(success);

            success = await Fixture.BusinessApi.InPipe.ExecuteFromQueueAsync();
            Assert.True(success);
        }

        [Fact]
        public async Task T10WorkFlowMethodIsInAsyncManagerQueue()
        {
            const string expectedEmailAddress = "john.doe@example.com";
            var requestId = await Fixture.Client.OnBoardCustomerAsync(expectedEmailAddress);
            var success = await Fixture.AsyncManager.CallFirstInQueue();
            Assert.True(success);
            success = await Fixture.WorkFlow.InPipe.ExecuteFromQueueAsync();
            Assert.True(success);

            success = await Fixture.AsyncManager.CallFirstInQueue();
            Assert.True(success);

            success = await Fixture.BusinessApi.InPipe.ExecuteFromQueueAsync();
            Assert.True(success);

            success = await Fixture.AsyncManager.CallFirstInQueue();
            Assert.True(success);

            success = await Fixture.BusinessApi.InPipe.ExecuteFromQueueAsync();
            Assert.True(success);

            success = await Fixture.AsyncManager.CallFirstInQueue();
            Assert.True(success);

            success = await Fixture.BusinessApi.InPipe.ExecuteFromQueueAsync();
            Assert.True(success);

            success = await Fixture.AsyncManager.CallFirstInQueue();
            Assert.True(success);

            success = await Fixture.BusinessApi.InPipe.ExecuteFromQueueAsync();
            Assert.True(success);

            success = await Fixture.AsyncManager.CallFirstInQueue();
            Assert.True(success);

            success = await Fixture.BusinessApi.InPipe.ExecuteFromQueueAsync();
            Assert.True(success);

            Assert.True(Fixture.AsyncManager.RequestQueue.TryPeek(out var actualRequestId));
            Assert.Equal(requestId, actualRequestId);
        }

        [Fact]
        public async Task T11WorkFlowMethodIsActivatedFromQueue()
        {
            const string expectedEmailAddress = "john.doe@example.com";
            await Fixture.Client.OnBoardCustomerAsync(expectedEmailAddress);
            var success = await Fixture.AsyncManager.CallFirstInQueue();
            Assert.True(success);
            success = await Fixture.WorkFlow.InPipe.ExecuteFromQueueAsync();
            Assert.True(success);

            success = await Fixture.AsyncManager.CallFirstInQueue();
            Assert.True(success);

            success = await Fixture.BusinessApi.InPipe.ExecuteFromQueueAsync();
            Assert.True(success);

            success = await Fixture.AsyncManager.CallFirstInQueue();
            Assert.True(success);
        }

        [Fact]
        public async Task T12WorkFlowStoresRequest()
        {
            const string expectedEmailAddress = "john.doe@example.com";
            var requestId = await Fixture.Client.OnBoardCustomerAsync(expectedEmailAddress);

            var success = await Fixture.AsyncManager.CallFirstInQueue();
            Assert.True(success);
            success = await Fixture.WorkFlow.InPipe.ExecuteFromQueueAsync();
            Assert.True(success);

            success = await Fixture.AsyncManager.CallFirstInQueue();
            Assert.True(success);
            success = await Fixture.BusinessApi.InPipe.ExecuteFromQueueAsync();
            Assert.True(success);

            success = await Fixture.AsyncManager.CallFirstInQueue();
            Assert.True(success);
            success = await Fixture.BusinessApi.InPipe.ExecuteFromQueueAsync();
            Assert.True(success);

            success = await Fixture.AsyncManager.CallFirstInQueue();
            Assert.True(success);
            success = await Fixture.BusinessApi.InPipe.ExecuteFromQueueAsync();
            Assert.True(success);

            success = await Fixture.AsyncManager.CallFirstInQueue();
            Assert.True(success);
            success = await Fixture.BusinessApi.InPipe.ExecuteFromQueueAsync();
            Assert.True(success);

            success = await Fixture.AsyncManager.CallFirstInQueue();
            Assert.True(success);
            success = await Fixture.BusinessApi.InPipe.ExecuteFromQueueAsync();
            Assert.True(success);

            success = await Fixture.AsyncManager.CallFirstInQueue();
            Assert.True(success);

            Assert.Collection(Fixture.WorkFlow.InPipe.Queue, i =>
            {
                Assert.True(Fixture.AsyncManager.Executions.TryGetValue(i.ExecutionId, out var execution));
                Assert.Equal(requestId, execution.RequestId);
            });
        }

        [Fact]
        public async Task T13WorkFlowMethodIsActivatedFromQueue()
        {
            const string expectedEmailAddress = "john.doe@example.com";
            await Fixture.Client.OnBoardCustomerAsync(expectedEmailAddress);

            var success = await Fixture.AsyncManager.CallFirstInQueue();
            Assert.True(success);
            success = await Fixture.WorkFlow.InPipe.ExecuteFromQueueAsync();
            Assert.True(success);

            success = await Fixture.AsyncManager.CallFirstInQueue();
            Assert.True(success);
            success = await Fixture.BusinessApi.InPipe.ExecuteFromQueueAsync();
            Assert.True(success);

            success = await Fixture.AsyncManager.CallFirstInQueue();
            Assert.True(success);
            success = await Fixture.BusinessApi.InPipe.ExecuteFromQueueAsync();
            Assert.True(success);

            success = await Fixture.AsyncManager.CallFirstInQueue();
            Assert.True(success);
            success = await Fixture.BusinessApi.InPipe.ExecuteFromQueueAsync();
            Assert.True(success);

            success = await Fixture.AsyncManager.CallFirstInQueue();
            Assert.True(success);
            success = await Fixture.BusinessApi.InPipe.ExecuteFromQueueAsync();
            Assert.True(success);

            success = await Fixture.AsyncManager.CallFirstInQueue();
            Assert.True(success);
            success = await Fixture.BusinessApi.InPipe.ExecuteFromQueueAsync();
            Assert.True(success);

            success = await Fixture.AsyncManager.CallFirstInQueue();
            Assert.True(success);

            success = await Fixture.WorkFlow.InPipe.ExecuteFromQueueAsync();
            Assert.True(success);
        }

        [Fact]
        public async Task T20ClientGotResponse()
        {
            const string expectedEmailAddress = "john.doe@example.com";
            await Fixture.Client.OnBoardCustomerAsync(expectedEmailAddress);

            var success = await Fixture.AsyncManager.CallFirstInQueue();
            Assert.True(success);
            success = await Fixture.WorkFlow.InPipe.ExecuteFromQueueAsync();
            Assert.True(success);

            success = await Fixture.AsyncManager.CallFirstInQueue();
            Assert.True(success);
            success = await Fixture.BusinessApi.InPipe.ExecuteFromQueueAsync();
            Assert.True(success);

            success = await Fixture.AsyncManager.CallFirstInQueue();
            Assert.True(success);
            success = await Fixture.BusinessApi.InPipe.ExecuteFromQueueAsync();
            Assert.True(success);

            success = await Fixture.AsyncManager.CallFirstInQueue();
            Assert.True(success);
            success = await Fixture.BusinessApi.InPipe.ExecuteFromQueueAsync();
            Assert.True(success);

            success = await Fixture.AsyncManager.CallFirstInQueue();
            Assert.True(success);
            success = await Fixture.BusinessApi.InPipe.ExecuteFromQueueAsync();
            Assert.True(success);

            success = await Fixture.AsyncManager.CallFirstInQueue();
            Assert.True(success);
            success = await Fixture.BusinessApi.InPipe.ExecuteFromQueueAsync();
            Assert.True(success);

            success = await Fixture.AsyncManager.CallFirstInQueue();
            Assert.True(success);
            success = await Fixture.WorkFlow.InPipe.ExecuteFromQueueAsync();
            Assert.True(success);

            success = await Fixture.AsyncManager.CallFirstInQueue();
            Assert.True(success);
            success = await Fixture.BusinessApi.InPipe.ExecuteFromQueueAsync();
            Assert.True(success);

            success = await Fixture.AsyncManager.CallFirstInQueue();
            Assert.True(success);
            success = await Fixture.BusinessApi.InPipe.ExecuteFromQueueAsync();
            Assert.True(success);

            success = await Fixture.AsyncManager.CallFirstInQueue();
            Assert.True(success);
            success = await Fixture.BusinessApi.InPipe.ExecuteFromQueueAsync();
            Assert.True(success);

            success = await Fixture.AsyncManager.CallFirstInQueue();
            Assert.True(success);
            success = await Fixture.BusinessApi.InPipe.ExecuteFromQueueAsync();
            Assert.True(success);

            success = await Fixture.AsyncManager.CallFirstInQueue();
            Assert.True(success);
            success = await Fixture.BusinessApi.InPipe.ExecuteFromQueueAsync();
            Assert.True(success);

            success = await Fixture.AsyncManager.CallFirstInQueue();
            Assert.True(success);
            success = await Fixture.WorkFlow.InPipe.ExecuteFromQueueAsync();
            Assert.True(success);

            success = await Fixture.AsyncManager.CallFirstInQueue();
            Assert.True(success);
            success = await Fixture.BusinessApi.InPipe.ExecuteFromQueueAsync();
            Assert.True(success);

            success = await Fixture.AsyncManager.CallFirstInQueue();
            Assert.True(success);
            success = await Fixture.BusinessApi.InPipe.ExecuteFromQueueAsync();
            Assert.True(success);

            success = await Fixture.AsyncManager.CallFirstInQueue();
            Assert.True(success);
            success = await Fixture.WorkFlow.InPipe.ExecuteFromQueueAsync();
            Assert.True(success);

            success = await Fixture.AsyncManager.CallFirstInQueue();
            Assert.True(success);
            success = await Fixture.WorkFlow.InPipe.ExecuteFromQueueAsync();
            Assert.True(success);

            success = await Fixture.AsyncManager.CallFirstInQueue();
            Assert.True(success);
            success = await Fixture.BusinessApi.InPipe.ExecuteFromQueueAsync();
            Assert.True(success);

            success = await Fixture.AsyncManager.CallFirstInQueue();
            Assert.True(success);
            success = await Fixture.WorkFlow.InPipe.ExecuteFromQueueAsync();
            Assert.True(success);

            success = await Fixture.AsyncManager.CallFirstInQueue();
            Assert.True(success);

            Assert.NotNull(Fixture.Client.Customer);
        }

        [Fact]
        public async Task T31WorkFlowException()
        {
            var requestId = await Fixture.Client.OnBoardCustomerAsync(null);
            await CompleteExecutionAsync(requestId);

            Assert.NotNull(Fixture.Client.Exception);
            Output.WriteLine(Fixture.Client.Exception.ToString());
            Assert.IsType<ArgumentException>(Fixture.Client.Exception);
        }

        [Fact]
        public async Task T32BusinessApiException()
        {
            var requestId = await Fixture.Client.OnBoardCustomerAsync(BusinessApi.FailForThisEmailAddress);
            await CompleteExecutionAsync(requestId);

            Assert.NotNull(Fixture.Client.Exception);
            Output.WriteLine(Fixture.Client.Exception.ToString());
            Assert.IsType<SubRequestException>(Fixture.Client.Exception);
        }
    }
}
