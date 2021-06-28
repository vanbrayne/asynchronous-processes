using System;
using System.Threading;
using System.Threading.Tasks;
using PoC.SystemTest.WorkFlowServer.Experiment.Example;

namespace PoC.SystemTest.WorkFlowServer.Experiment.Capabilities.CustomerInformationMgmt
{
    public class PersonService : IPersonService
    {
        private readonly CreatePersonProcess _createProcess;

        public PersonService()
        {
            _createProcess = new CreatePersonProcess(null, null);
        }
        /// <inheritdoc />
        public Task<Person> ReadAsync(string id, CancellationToken token = new CancellationToken())
        {
            return Task.FromResult(new Person());
        }

        /// <inheritdoc />
        public async Task<string> CreateAsync(Person item, CancellationToken cancellationToken = default)
        {
            // TODO: Always use latest version
            var person = await _createProcess.ExecuteAsync($"{item.EmailAddress}", cancellationToken, item.PersonalNumber, item.EmailAddress);
            return person.Id;
        }

        /// <inheritdoc />
        public Task<Person> GetByPersonalNumberAsync(string personalNumber, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task<Person> GetOfficialInformationAsync(Person person, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public async Task<Person> AskUserToFillInDetailsAsync(Person person, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public async Task<bool> ValidateAsync(Person person, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}