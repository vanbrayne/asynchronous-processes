using System.Threading;
using System.Threading.Tasks;
using Nexus.Link.Libraries.Crud.Web.RestClient;
using Nexus.Link.Libraries.Web.RestClientHelper;
using PoC.Example.Abstract.Capabilities.Common;
using PoC.Example.Abstract.Capabilities.CustomerInformationMgmt;

namespace PoC.API.RestClients.CustomerInformationMgmt
{
    public class PersonRestClient : CrudRestClient<Person, string>, IPersonService
    {
        public PersonRestClient(IHttpSender httpSender)
            : base(httpSender)
        {
        }

        /// <inheritdoc />
        public Task<Person> GetByPersonalNumberAsync(Person person, CancellationToken cancellationToken = default)
        {
            return PostAndReturnCreatedObjectAsync<Person>($"ByPersonalNumber", person, null, cancellationToken);
        }

        /// <inheritdoc />
        public Task<Person> GetOfficialInformationAsync(Person person, CancellationToken cancellationToken = default)
        {
            return PostAndReturnCreatedObjectAsync($"OfficialInformation", person, null, cancellationToken);
        }

        /// <inheritdoc />
        public Task<Person> AskUserToFillInDetailsAsync(Person person, CancellationToken cancellationToken)
        {
            return PostAndReturnCreatedObjectAsync("AskForDetails", person, null, cancellationToken);
        }

        /// <inheritdoc />
        public Task<bool> ValidateAsync(Person person, CancellationToken cancellationToken)
        {
            return PostAsync<bool, Person>("Validate", person, null, cancellationToken);
        }
    }
}