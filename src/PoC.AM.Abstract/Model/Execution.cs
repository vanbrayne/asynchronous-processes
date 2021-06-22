using System;
using System.Threading.Tasks;

namespace PoC.AM.Abstract.Model
{
    public delegate Task<string> RemoteMethodDelegate(Headers headers);
    public enum RequestStateEnum
    {
        Waiting,
        Running,
        Completed
    }

    public class Execution : ILockable
    {
        public Guid Id { get; set; }
        public Guid RequestId { get; set; }
        public RequestStateEnum State { get; set; }
        public RemoteMethodDelegate RemoteMethod { get; set; }
        public string ResponseAsJson { get; set; }
        public Exception Exception { get; set; }

        // Convenience methods
        public bool HasCompleted => State == RequestStateEnum.Completed;
        public bool IsWaiting => State == RequestStateEnum.Waiting;
        public bool HasFailed => HasCompleted && Exception != null;

        /// <inheritdoc />
        public LockWithTimeout LockWithTimeout { get; set; } = new LockWithTimeout();
    }
}