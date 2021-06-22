using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PoC.AM.Abstract.Model
{
    public delegate Task CallbackDelegate(Guid requestId, string resultAsString, Exception exception = null);

    public class Request : ILockable
    {

        private readonly IDictionary<Guid, Execution> _executions;

        public Request(IDictionary<Guid, Execution> executions)
        {
            _executions = executions;
        }
        public Guid Id { get; set; }
        public string Description { get; set; }
        public RemoteMethodDelegate RemoteMethod { get; set; }
        public CallbackDelegate Callback { get; set; }
        public Guid? LatestExecutionId { get; set; }
        public Guid? CallbackRequestId { get; set; }
        public Guid? ParentRequestId { get; set; }

        // Convenience methods
        public RequestStateEnum State => LatestExecution?.State ?? RequestStateEnum.Waiting;
        public bool HasCompleted => State == RequestStateEnum.Completed;
        public bool IsWaiting => State == RequestStateEnum.Waiting;
        public bool HasFailed => HasCompleted && LatestExecution?.Exception != null;
        public Execution LatestExecution
        {
            get
            {
                if (!LatestExecutionId.HasValue) return null;
                return !_executions.TryGetValue(LatestExecutionId.Value, out var execution) ? null : execution;
            }
        }

        /// <inheritdoc />
        public LockWithTimeout LockWithTimeout { get; } = new LockWithTimeout();

        public string StateAsString()
        {
            var result = $"{State}";
            if (HasCompleted && HasFailed) result += " (failed)";
            return result;
        }
    }
}