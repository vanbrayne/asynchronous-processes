using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Nexus.Link.Libraries.Core.Application;
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
            FulcrumApplication.Context.AsyncContext.CurrentProcessId = processId;
            throw new NotImplementedException();
        }
        
        public async Task<T> ExecuteAsync(int majorVersionNumber, string instanceTitle, CancellationToken cancellationToken, params object[] arguments)
        {
            var instance = CreateInstance(majorVersionNumber, instanceTitle, arguments);
            // TODO: Verify arguments with Parameters
            // TODO: Set the arguments: Parameters.SetArguments(arguments);
            var version = ProcessVersions.GetVersion(majorVersionNumber);
            try
            {
                var result = await version.Method(instance, cancellationToken);
                // TODO: What to do, now that the instance has completed, e.g. DB ProcessInstance.FinishedAt = DateTimeOffset.NowUtc.
                return result;
            }
            catch (PostponeException e)
            {
                // TODO: Postpone
            }
            catch (Exception e)
            {
                // TODO: Fatal error
            }
        }

        private ProcessInstance<T> CreateInstance(in int version, string instanceTitle, object[] arguments)
        {
            // TODO: Set start time
            // TODO: Set correlation id
            // TODO: Set arguments

            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}