using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PoC.Example.Abstract.Capabilities.Common;
using PoC.Example.Abstract.Capabilities.CommunicationMgmt;
using PoC.Example.Abstract.Capabilities.CustomerInformationMgmt;
using PoC.Example.Abstract.Capabilities.CustomerOnboardingMgmt;
using PoC.Example.Persistence;
using IPersonService = PoC.Example.Abstract.Capabilities.CustomerInformationMgmt.IPersonService;

namespace PoC.Example.Capabilities.CustomerInformationMgmt
{
    public class PersonService : IPersonService
    {
        private readonly ICustomerInformationMgmtCapability _capability;
        private readonly IPersonTable _personTable;
        private Person _player1;
        private Person _player2;
        private Person _player3;

        public PersonService(ICustomerInformationMgmtCapability capability, IPersonTable personTable, ICommunicationMgmtCapability communicationMgmtCapability)
        {
            _capability = capability;
            _personTable = personTable;
            _player1 = new Person
            {
                Name = "Player1",
                PersonalNumber = "111111-1111"
            };
            _player1  = personTable.CreateAndReturnAsync(_player1).Result;
            _player2 = new Person
            {
                Name = "Player2",
                PersonalNumber = "222222-2222"
            };
            _player2 = personTable.CreateAndReturnAsync(_player2).Result;
            _player3 = new Person
            {
                Name = "Player3",
                PersonalNumber = "333333-3333"
            };
            _player3 = personTable.CreateAndReturnAsync(_player3).Result;
        }

        /// <inheritdoc />
        public async Task<Person> CreateAndReturnAsync(Person person, CancellationToken cancellationToken = default)
        {
            return await _personTable.CreateAndReturnAsync(person, cancellationToken);
        }

        /// <inheritdoc />
        public Task<Person> ReadAsync(string id, CancellationToken token = new CancellationToken())
        {
            return _personTable.ReadAsync(id, token);
        }

        /// <inheritdoc />
        public Task<Person> GetByPersonalNumberAsync(Person person, CancellationToken cancellationToken = default)
        {
            return _personTable.GetByPersonalNumberAsync(person, cancellationToken);
        }

        /// <inheritdoc />
        public Task<Person> GetOfficialInformationAsync(Person person, CancellationToken cancellationToken = default)
        {
            person.Name = "John Doe";
            return Task.FromResult(person);
        }

        /// <inheritdoc />
        public Task<Person> AskUserToFillInDetailsAsync(Person person, CancellationToken cancellationToken)
        {
            person.FavoriteFootballPlayers.Add(_player1);
            person.FavoriteFootballPlayers.Add(_player3);
            return Task.FromResult(person);
        }
        
        private bool _validateResult;
        /// <inheritdoc />
        public Task<bool> ValidateAsync(Person person, CancellationToken cancellationToken)
        {
            _validateResult = !_validateResult;
            return Task.FromResult(_validateResult);
        }

        /// <inheritdoc />
        public Task<IEnumerable<Person>> ReadAllAsync(int limit = 2147483647, CancellationToken token = new CancellationToken())
        {
            return _personTable.ReadAllAsync(limit, token);
        }
    }
}