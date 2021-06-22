using System;
using System.Threading.Tasks;
using PoC.AM.Abstract.Exceptions;
using PoC.SystemTest.BusinessApiServer;
using Xunit;
using Xunit.Abstractions;

namespace PoC.SystemTest
{
    public class SynchronousTest : SystemTestBase
    {
        public SynchronousTest(SystemTestFixture fixture, ITestOutputHelper output)
            : base(fixture, output)
        {
        }

        [Fact]
        public async Task T01SunshineCase()
        {
            const string expectedEmailAddress = "john.doe@example.com";
            var customer = await Fixture.Client.OnBoardCustomerSynchronouslyAsync(expectedEmailAddress);
            Assert.NotNull(customer);
            Assert.Equal(expectedEmailAddress, customer.EmailAddress);
        }

        [Fact]
        public async Task T02WorkFlowException()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => Fixture.Client.OnBoardCustomerSynchronouslyAsync(null));
        }

        [Fact]
        public async Task T03BusinessApiException()
        {
            var e = await Assert.ThrowsAsync<SubRequestException>(() => Fixture.Client.OnBoardCustomerSynchronouslyAsync(BusinessApi.FailForThisEmailAddress));
            Output.WriteLine(e.ToString());
        }
    }
}
