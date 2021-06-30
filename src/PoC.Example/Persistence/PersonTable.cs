using System.Threading;
using System.Threading.Tasks;
using Nexus.Link.Libraries.Crud.MemoryStorage;
using Nexus.Link.Libraries.Crud.Model;
using PoC.Example.Abstract.Capabilities.Common;
using PoC.Example.Abstract.Capabilities.CustomerInformationMgmt;

namespace PoC.Example.Persistence
{
    public class PersonTable : CrudMemory<Person, string>, IPersonTable
    {
        /// <inheritdoc />
        public Task<Person> GetByPersonalNumberAsync(Person person, CancellationToken cancellationToken = default)
        {
            return FindUniqueAsync(new SearchDetails<Person>(new {PersonalNumber = person.PersonalNumber}), cancellationToken);
        }
    }
}