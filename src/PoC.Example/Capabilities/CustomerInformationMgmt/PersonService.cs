using System;
using System.Threading;
using System.Threading.Tasks;
using PoC.Example.Abstract.Capabilities.CustomerInformationMgmt;
using PoC.Example.Example;
using PoC.LinkLibraries.LibraryCode;

namespace PoC.Example.Capabilities.CustomerInformationMgmt
{
    public class PersonService : IPersonService
    {
        private readonly ProcessDefinition<Person> _createPersonProcess;

        public PersonService(ProcessDefinition<Person> createPersonProcess)
        {
            _createPersonProcess = createPersonProcess;
        }
        /// <inheritdoc />
        public Task<Person> ReadAsync(string id, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new Person());
        }

        /// <inheritdoc />
        public async Task<string> CreateAsync(Person item, CancellationToken cancellationToken = default)
        {
            // TODO: Always use latest version
            var person = await _createPersonProcess.ExecuteAsync($"{item.EmailAddress}", cancellationToken, item.PersonalNumber, item.EmailAddress);
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
        public async Task<Person> AskUserToFillInDetailsAsync(string id, Person person, CancellationToken cancellationToken)
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