using System.Threading;
using System.Threading.Tasks;
using Nexus.Link.Libraries.Crud.Interfaces;
using PoC.Example.Abstract.Capabilities.Common;
using PoC.Example.Abstract.Capabilities.CustomerInformationMgmt;

namespace PoC.Example.Persistence
{
    public interface IPersonTable : ICreateAndReturn<Person, string>, IRead<Person, string>, IReadAll<Person, string>
    {
        Task<Person> GetByPersonalNumberAsync(Person person, CancellationToken cancellationToken = default);
    }
}