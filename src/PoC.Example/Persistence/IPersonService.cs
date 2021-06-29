using System.Threading;
using System.Threading.Tasks;
using Nexus.Link.Libraries.Crud.Interfaces;
using PoC.Example.Abstract.Capabilities.CustomerInformationMgmt;

namespace PoC.Example.Persistence
{
    public interface IPersonTable : ICreateAndReturn<Person, string>, IRead<Person, string>
    {
        Task<Person> GetByPersonalNumberAsync(string personalNumber, CancellationToken cancellationToken = default);
    }
}