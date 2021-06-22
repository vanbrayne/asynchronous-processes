using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using PoC.AM.Abstract.Model;

namespace PoC.AM.Abstract
{
    public interface IAsyncRequestResponseManagement
    {
        ConcurrentDictionary<Guid, Request> Requests { get; }
        Dictionary<Guid, Execution> Executions { get; }
        Queue<Guid> RequestQueue { get; }
        void Clear();
        Task<Guid> CreateAsyncRequestAsync(string description, RemoteMethodDelegate remoteMethod, CallbackDelegate callback = null);
        Task<Guid> CreateAsyncRequestAsync(string description, Guid parentExecutionId, RemoteMethodDelegate remoteMethod);
        Task<bool> CallFirstInQueue();
        Task<Execution> GetLockedExecutionAsync(Guid executionId, TimeSpan lockTimeSpan);
        Task UnlockRequestExecutionAsync(Guid executionId, Guid lockId);
        Task CreateResponse(Guid executionId, string responseAsJson);
        Task CreateException(Guid executionId, Exception exception);

    }

    public interface IAsyncContextManagement
    {
        Dictionary<Guid, ExecutionContext> ExecutionContexts { get; }
        Task<ExecutionContext> GetExecutionContextAsync(Guid executionId, int version);
        Task AddSubRequestAsync(Guid executionId, string identifier, SubRequest subRequest);
        Task<string> GetStatusAsStringAsync(Guid requestId);
    }

    public interface IAsyncManagement : IAsyncRequestResponseManagement, IAsyncContextManagement
    {
    }
}