using System.Threading;
using System.Threading.Tasks;
using Nexus.Link.Libraries.Crud.Interfaces;

namespace PoC.SystemTest.WorkFlowServer.Experiment
{
    public interface IPersonService : IRead<Person, string>, ICreate<Person, string>
    {
        Task<Person> GetByPersonalNumberAsync(string personalNumber, CancellationToken cancellationToken = default);

        Task<Person> InitializePerson(string personalNumber, string emailAddress,
            CancellationToken cancellationToken = default);
    }
}