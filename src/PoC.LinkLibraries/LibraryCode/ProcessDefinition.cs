using System;
using System.Threading;
using System.Threading.Tasks;
using Nexus.Link.Libraries.Core.Assert;
using PoC.AM.Abstract.Exceptions;

namespace PoC.LinkLibraries.LibraryCode
{
    public abstract class ProcessDefinition<T> : IDisposable, IProcessDefinition<T>
    {
        public string Id { get; }
        public string Title { get; }
        public ProcessVersionCollection<T> ProcessVersions { get; }

        protected ProcessDefinition(string title, string id)
        {
            Title = title;
            Id = id;
            ProcessVersions = new ProcessVersionCollection<T>(this);
        }

        public Task<T> ExecuteAsync(string instanceTitle, CancellationToken cancellationToken, params object[] arguments)
        {
            // TODO: If this is an instance that already exists, find the corresponding version.
            ProcessVersion<T> version;
            if (false)//FulcrumApplication.Context.AsyncInstanceId != null)
            {

            }
            else
            {
                version = ProcessVersions.GetLatestVersion();
            }

            return ExecuteAsync(version, instanceTitle, cancellationToken, arguments);
        }

        protected async Task<T> ExecuteAsync(ProcessVersion<T> version, string instanceTitle, CancellationToken cancellationToken, params object[] arguments)
        {
            // TODO: Verify arguments with Parameters

            var methodInfo = version.Type.GetMethod(nameof(ProcessInstance<T>.CreateInstance));
            // TODO: Proper error message
            InternalContract.RequireNotNull(methodInfo, null, $"Error message");
            var factoryMethod = (Func<ProcessVersion<T>, string, object[], ProcessInstance<T>>)Delegate.CreateDelegate(
                typeof(Func<ProcessVersion<T>, string, object[], ProcessInstance<T>>), methodInfo!);

            var instance = factoryMethod(version, instanceTitle, arguments);
            try
            {
                var result = await instance.ExecuteAsync(cancellationToken);
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

        /// <inheritdoc />
        public override string ToString() => $"{Title} ({Id})";

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}