using System.Threading;
using System.Threading.Tasks;

namespace PoC.LinkLibraries.LibraryCode
{
    public interface IProcessDefinition<T>
    {
        Task<T> ExecuteAsync(string instanceTitle, CancellationToken cancellationToken, params object[] arguments);
    }
}