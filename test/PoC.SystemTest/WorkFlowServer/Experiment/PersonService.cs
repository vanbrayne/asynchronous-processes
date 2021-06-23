using System;
using System.Threading;
using System.Threading.Tasks;

namespace PoC.SystemTest.WorkFlowServer.Experiment
{
    public class PersonService : IPersonService
    {
        /// <inheritdoc />
        public Task<Person> ReadAsync(string id, CancellationToken token = new CancellationToken())
        {
            return Task.FromResult(new Person());
        }

        /// <inheritdoc />
        public Task<string> CreateAsync(Person item, CancellationToken token = new CancellationToken())
        {
            //var process = new CreatePersonProcess();
            //process.Execute(item);
            return Task.FromResult("done");
        }

        /// <inheritdoc />
        public Task<Person> GetByPersonalNumberAsync(string personalNumber, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public async Task<Person> InitializePerson(string personalNumber, string emailAddress, CancellationToken cancellationToken = default)
        {
            var process = new InitializePersonProcess(null);
            return await process.ExecuteAsync(1, emailAddress, cancellationToken, personalNumber, emailAddress);
        }
    }
}