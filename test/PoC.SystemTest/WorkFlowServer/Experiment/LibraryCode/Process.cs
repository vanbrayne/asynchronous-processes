using System;
using System.Threading;
using System.Threading.Tasks;
using PoC.AM.Abstract.Exceptions;

namespace PoC.SystemTest.WorkFlowServer.Experiment.LibraryCode
{
    public abstract class Process<T> : IDisposable
    {
        protected ProcessVersionCollection<T> ProcessVersions { get;  } = new ProcessVersionCollection<T>();
        public string ProcessName { get; }
        public string ProcessId { get; }

        protected Process(string processName, string processId)
        {
            ProcessName = processName;
            ProcessId = processId;
        }
        
        public async Task<T> ExecuteAsync(int majorVersionNumber, string instanceTitle, CancellationToken cancellationToken, params object[] arguments)
        {
            // TODO: Verify arguments with Parameters
            var version = ProcessVersions.GetVersion(majorVersionNumber);
            var instance = new ProcessInstance<T>(version, instanceTitle, arguments);
            try
            {
                var result = await version.Method(instance, cancellationToken);
                // TODO: What to do, now that the instance has completed, e.g. DB ProcessInstance.FinishedAt = DateTimeOffset.NowUtc.
                return result;
            }
            catch (PostponeException e)
            {
                // TODO: Postpone
                throw;
            }
            catch (Exception e)
            {
                // TODO: Fatal error
                throw;
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}