using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using PoC.AM.Abstract;
using PoC.AM.Abstract.Exceptions;
using PoC.AM.Abstract.Model;
using PoC.LinkLibraries;
using ExecutionContext = PoC.AM.Abstract.Model.ExecutionContext;

namespace PoC.AM
{

    public class AsyncManager : IAsyncManagement
    {
        public ConcurrentDictionary<Guid, Request> Requests { get; } = new ConcurrentDictionary<Guid, Request>();
        public Dictionary<Guid, Execution> Executions { get; } = new Dictionary<Guid, Execution>();
        public Dictionary<Guid, ExecutionContext> ExecutionContexts { get; } = new Dictionary<Guid, ExecutionContext>();
        public Queue<Guid> RequestQueue { get; } = new Queue<Guid>();

        /// <inheritdoc />
        public void Clear()
        {
            Requests.Clear();
            RequestQueue.Clear();
            Executions.Clear();
            ExecutionContexts.Clear();
        }

        /// <inheritdoc />
        public Task<Guid> CreateAsyncRequestAsync(string description, RemoteMethodDelegate remoteMethod, CallbackDelegate callback)
        {
            var request = new Request(Executions)
            {
                Id = Guid.NewGuid(),
                Description = description,
                RemoteMethod = remoteMethod,
                Callback = callback
            };
            Requests[request.Id] = request;
            RequestQueue.Enqueue(request.Id);
            return Task.FromResult(request.Id);
        }

        /// <inheritdoc />
        public Task<Guid> CreateAsyncRequestAsync(string description, Guid parentExecutionId, RemoteMethodDelegate remoteMethod)
        {
            if (!Executions.TryGetValue(parentExecutionId, out var parentExecution))
            {
                throw new ProgrammersErrorException($"Could not find parent execution id {parentExecutionId}.");
            }
            var request = new Request(Executions)
            {
                Id = Guid.NewGuid(),
                Description = description,
                ParentRequestId = parentExecution.RequestId,
                RemoteMethod = remoteMethod
            };
            Requests[request.Id] = request;
            RequestQueue.Enqueue(request.Id);
            return Task.FromResult(request.Id);
        }

        public async Task<bool> CallFirstInQueue()
        {
            if (!RequestQueue.TryDequeue(out var requestId)) return false;
            if (!Requests.ContainsKey(requestId)) return true;
            var request = Requests[requestId];
            var requestLockId = await request.WaitForLockAsync();
            try
            {
                if (request.HasCompleted) return true;
                request.LatestExecutionId ??= CreateExecution(requestId);
                var headers = new Headers(request.LatestExecutionId);
                await request.RemoteMethod(headers);
            }
            catch (AcceptedException)
            {
                // Expected to be accepted
            }
            finally
            {
                request.Unlock(requestLockId);
            }

            return true;
        }

        private Guid? CreateExecution(Guid requestId)
        {
            if (!Requests.TryGetValue(requestId, out var request)) return null;

            if (!request.IsWaiting) throw new PostponeException();
            var execution = new Execution
            {
                Id = Guid.NewGuid(),
                State = RequestStateEnum.Running,
                RequestId = requestId,
                RemoteMethod = request.RemoteMethod
            };
            Executions.Add(execution.Id, execution);
            request.LatestExecutionId = execution.Id;
            return execution.Id;
        }

        /// <inheritdoc />
        public async Task CreateResponse(Guid executionId, string responseAsJson)
        {
            if (!Executions.TryGetValue(executionId, out var execution)) return;
            var executionLockId = await execution.WaitForLockAsync();
            try
            {
                if (!Requests.TryGetValue(execution.RequestId, out var request))
                {
                    throw new ProgrammersErrorException($"The execution {executionId} existed, but not its request {execution.RequestId}");
                }
                execution.ResponseAsJson = responseAsJson;
                execution.State = RequestStateEnum.Completed;
                if (request.Callback != null)
                {
                    request.CallbackRequestId = await CreateAsyncRequestAsync(
                        $"Callback for ${request.Description}",
                        headers =>
                    {
                        request.Callback(request.Id, responseAsJson);
                        return Task.FromResult(responseAsJson);
                    }, null);
                }
                else
                {
                    if (request.ParentRequestId != null) RequestQueue.Enqueue(request.ParentRequestId.Value);
                }
            }
            finally
            {
                execution.Unlock(executionLockId);
            }
        }

        /// <inheritdoc />
        public async Task CreateException(Guid executionId, Exception exception)
        {
            if (!Executions.TryGetValue(executionId, out var execution)) return;
            var executionLockId = await execution.WaitForLockAsync();
            try
            {
                if (!Requests.TryGetValue(execution.RequestId, out var request))
                {
                    throw new ProgrammersErrorException($"The execution {executionId} existed, but not its request {execution.RequestId}");
                }
                execution.State = RequestStateEnum.Completed;
                execution.Exception = exception;
                if (request.Callback != null)
                {
                    request.CallbackRequestId = await CreateAsyncRequestAsync(
                        $"Callback for ${request.Description}",
                        headers =>
                    {
                        request.Callback(request.Id, null, exception);
                        return Task.FromResult(null as string);
                    }, null);
                }
                else
                {
                    if (request.ParentRequestId != null) RequestQueue.Enqueue(request.ParentRequestId.Value);
                }
            }
            finally
            {
                execution.Unlock(executionLockId);
            }
        }

        /// <inheritdoc />
        public async Task<ExecutionContext> GetExecutionContextAsync(Guid executionId, int version)
        {
            if (!ExecutionContexts.TryGetValue(executionId, out var context))
            {
                return await CreateExecutionContextAsync(executionId, version);
            }
            var contextLockId = await context.WaitForLockAsync();
            try
            {

                foreach (var subRequest in context.SubRequests.Values)
                {
                    UpdateResponseValue(subRequest);
                }

                return context.AsCopy();
            }
            finally
            {
                context.Unlock(contextLockId);
            }

            #region Local methods
            void UpdateResponseValue(SubRequest subRequest)
            {
                if (subRequest.HasCompleted) return;
                if (!subRequest.RequestId.HasValue) return;
                try
                {
                    if (!Requests.TryGetValue(subRequest.RequestId.Value, out var request))
                    {
                        throw new NotImplementedException("Can this happen? Maybe due to expiration time for a request?");
                    }
                    lock (request)
                    {
                        if (!request.HasCompleted) throw new PostponeException();
                        if (!request.LatestExecutionId.HasValue) throw new ProgrammersErrorException($"The request {request.Id} is completed, so it should have a {nameof(request.LatestExecutionId)}.");
                        if (request.HasFailed)
                        {
                            subRequest.ExceptionTypeName = request.LatestExecution.Exception.GetType().FullName;
                        }
                        else
                        {
                            if (request.LatestExecution == null)
                            {
                                throw new ProgrammersErrorException("No reason this shouldn't exist.");
                            }

                            subRequest.ResultValueAsJson = request.LatestExecution.ResponseAsJson;
                        }

                    }
                    subRequest.HasCompleted = true;
                }
                catch (PostponeException)
                {
                    // The value is not ready yet.
                };
            }
            #endregion

        }

        private async Task<ExecutionContext> CreateExecutionContextAsync(Guid executionId, int version)
        {
            if (!Executions.TryGetValue(executionId, out var execution)) return null;

            var requestLockId = await execution.WaitForLockAsync();
            try
            {
                if (execution.State != RequestStateEnum.Running) throw new PostponeException();
                var context = new ExecutionContext
                {
                    ExecutionId = execution.Id,
                    IsAsynchronous = true,
                    NextSubRequestNumber = 1,
                    Version = version
                };
                ExecutionContexts[execution.Id] = context;
                return context;
            }
            finally
            {
                execution.Unlock(requestLockId);
            }
        }

        /// <inheritdoc />
        public async Task<Execution> GetLockedExecutionAsync(Guid executionId, TimeSpan lockTimeSpan)
        {
            if (!Executions.TryGetValue(executionId, out var execution)) return null;
            await execution.WaitForLockAsync(lockTimeSpan, TimeSpan.Zero);
            return Copy(execution);

            #region Local methods
            static Execution Copy(Execution source)
            {
                // The method will result in a loop for JsonConvert, so we use the same reference for the method.
                var method = source.RemoteMethod;
                source.RemoteMethod = null;
                var result = source.AsCopy();
                source.RemoteMethod = method;
                result.RemoteMethod = method;
                // The LockWithTimeOut properties has no setters, so we have to copy it separately.
                result.LockWithTimeout = source.LockWithTimeout.AsCopy();
                return result;
            }
            #endregion
        }

        public Task UnlockRequestExecutionAsync(Guid executionId, Guid lockId)
        {
            if (!Executions.TryGetValue(executionId, out var execution)) return Task.CompletedTask;
            execution.Unlock(lockId);
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public async Task AddSubRequestAsync(Guid executionId, string identifier, SubRequest subRequest)
        {
            if (!ExecutionContexts.ContainsKey(executionId)) throw new ProgrammersErrorException($"Expected to find execution context for execution {executionId} for sub request {identifier}, {subRequest.Description}");
            var context = ExecutionContexts[executionId];

            var contextLockId = await context.WaitForLockAsync(TimeSpan.FromSeconds(1));
            try
            {
                if (!context.SubRequests.TryAdd(identifier, subRequest))
                    throw new ProgrammersErrorException(
                        $"Execution {executionId} unexpectedly already had sub request {identifier}, {subRequest.Description}");
            }
            finally
            {
                context.Unlock(contextLockId);
            }
        }

        /// <inheritdoc />
        public Task<string> GetStatusAsStringAsync(Guid requestId)
        {
            return GetStatusAsStringAsync($"{requestId}", requestId, Indentation);
        }

        private const string Indentation = "  ";

        private async Task<string> GetStatusAsStringAsync(string title, Guid requestId, string indentation)
        {
            if (!Requests.TryGetValue(requestId, out var request)) return "";
            var builder = new StringBuilder($"{title}: {request.StateAsString()}");
            if (request.LatestExecutionId != null)
            {
                var context = ExecutionContexts[request.LatestExecutionId.Value];
                var contextLockId = await context.WaitForLockAsync(TimeSpan.FromSeconds(1));
                try
                {
                    foreach (var subRequest in context.SubRequests.Values)
                    {
                        var subTitle = $"\r{indentation}{subRequest.Description}";
                        if (!subRequest.RequestId.HasValue)
                        {
                            builder.Append(subTitle);
                        }
                        else
                        {
                            var subStatus = await GetStatusAsStringAsync(subTitle, subRequest.RequestId.Value,
                                indentation + Indentation);
                            builder.Append(subStatus);
                        }
                    }
                }
                finally
                {
                    context.Unlock(contextLockId);
                }
            }

            return builder.ToString();
        }
    }
}