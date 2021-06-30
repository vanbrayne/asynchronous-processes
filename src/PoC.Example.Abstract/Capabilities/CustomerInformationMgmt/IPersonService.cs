using System.Threading;
using System.Threading.Tasks;
using Nexus.Link.Libraries.Crud.Interfaces;
using PoC.Example.Abstract.Capabilities.Common;

namespace PoC.Example.Abstract.Capabilities.CustomerInformationMgmt
{
    public interface IPersonService : ICreateAndReturn<Person, string>, IRead<Person, string>, IReadAll<Person, string>
    {
        Task<Person> GetByPersonalNumberAsync(Person person, CancellationToken cancellationToken = default);
        Task<Person> GetOfficialInformationAsync(Person person, CancellationToken cancellationToken = default);

        Task<Person> AskUserToFillInDetailsAsync(Person person, CancellationToken cancellationToken);
        Task<bool> ValidateAsync(Person person, CancellationToken cancellationToken);
    }
}