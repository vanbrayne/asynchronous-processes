using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PoC.SystemTest
{
    public class DemoTest : SystemTestBase
    {
        public DemoTest(SystemTestFixture fixture, ITestOutputHelper output) 
        :base(fixture, output)
        {
        }

        [Fact]
        public async Task CompleteExample()
        {
            const string expectedEmailAddress = "john.doe@example.com";
            var requestId = await Fixture.Client.OnBoardCustomerAsync(expectedEmailAddress);
            await CompleteExecutionAsync(requestId);

            Assert.NotNull(Fixture.Client.Customer);
        }

        [Fact]
        public async Task ServerException()
        {
            var requestId = await Fixture.Client.OnBoardCustomerAsync(null);
            await CompleteExecutionAsync(requestId);

            Assert.NotNull(Fixture.Client.Exception);
            Output.WriteLine(Fixture.Client.Exception.ToString());
        }
    }
}
