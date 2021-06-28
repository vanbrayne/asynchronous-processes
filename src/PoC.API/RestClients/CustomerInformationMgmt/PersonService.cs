using System.Threading;
using System.Threading.Tasks;
using Nexus.Link.Libraries.Crud.Web.RestClient;
using Nexus.Link.Libraries.Web.RestClientHelper;
using PoC.Example.Abstract.Capabilities.CustomerInformationMgmt;

namespace PoC.API.RestClients.CustomerInformationMgmt
{
    public class PersonService : CrudRestClient<Person, string>, IPersonService
    {
        public PersonService(HttpSender httpSender)
            : base(httpSender)
        {
        }

        /// <inheritdoc />
        public Task<Person> GetByPersonalNumberAsync(string personalNumber, CancellationToken cancellationToken = default)
        {
            return PostAsync<Person>($"?personalNumber={personalNumber}", null, cancellationToken);
        }

        /// <inheritdoc />
        public Task<Person> GetOfficialInformationAsync(Person person, CancellationToken cancellationToken = default)
        {
            return PostAndReturnCreatedObjectAsync("OfficialInformation", person, null, cancellationToken);
        }

        /// <inheritdoc />
        public Task<Person> AskUserToFillInDetailsAsync(string id, Person person, CancellationToken cancellationToken)
        {
            return PostAndReturnCreatedObjectAsync($"{id}/AskForDetails", person, null, cancellationToken);
        }

        /// <inheritdoc />
        public Task<bool> ValidateAsync(Person person, CancellationToken cancellationToken)
        {
            return PostAsync<bool, Person>($"{person.Id}/Validate", person, null, cancellationToken);
        }
    }
}