using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PoC.AM.Abstract;
using PoC.AM.Abstract.Exceptions;
using PoC.AM.Abstract.Model;

namespace PoC.LinkLibraries
{
    public class InPipe
    {
        private static readonly int MaxNumberOfItemsInQueue = 100;
        private static readonly TimeSpan ExecutionLockTimeSpan = TimeSpan.FromMinutes(2);
        private readonly IAsyncManagement _asyncManager;

        public InPipe(IAsyncManagement asyncManager)
        {
            _asyncManager = asyncManager;
        }

        public Queue<QueueItem> Queue { get; } = new Queue<QueueItem>();
        public async Task<ExecutionContext> PreInvokeAsync(Headers headers, int version = 0)
        {
            if (!headers.ExecutionId.HasValue) return new ExecutionContext();
            if (!headers.IsInternalCall)
            {
                if (Queue.Count > MaxNumberOfItemsInQueue) throw new PostponeException();
                EnqueueCall();
                throw new AcceptedException();
            }
            if (!headers.ExecutionId.HasValue) throw new ProgrammersErrorException("The header should have an ExecutionId at this stage.");

            return await _asyncManager.GetExecutionContextAsync(headers.ExecutionId.Value, version);

            #region Local methods
            void EnqueueCall()
            {
                var item = new QueueItem
                {
                    ExecutionId = headers.ExecutionId.Value
                };
                Queue.Enqueue(item);
            }
            #endregion
        }

        public Task PostInvokeAsync(Headers headers, ExecutionContext context)
        {
            if (!context.IsAsynchronous) return Task.CompletedTask;
            return _asyncManager.UnlockRequestExecutionAsync(context.ExecutionId, headers.ExecutionLockId);
        }

        public async Task<bool> ExecuteFromQueueAsync()
        {
            if (!Queue.TryDequeue(out var item)) return false;
            try
            {
                var execution = await _asyncManager.GetLockedExecutionAsync(item.ExecutionId, ExecutionLockTimeSpan);
                if (execution?.LockWithTimeout?.Id == null) return true;
                var headers = new Headers(item.ExecutionId)
                {
                    IsInternalCall = true,
                    ExecutionLockId = execution.LockWithTimeout.Id.Value,
                };
                string responseAsJson;
                try
                {
                    responseAsJson = await execution.RemoteMethod(headers);
                }
                catch (AcceptedException)
                {
                    return true;
                }
                catch (PostponeException)
                {
                    return true;
                }
                catch (Exception e)
                {
                    await _asyncManager.CreateException(execution.Id, e);
                    return true;
                }

                try
                {
                    await _asyncManager.CreateResponse(execution.Id, responseAsJson);
                }
                catch (PostponeException)
                {
                    throw new NotImplementedException("How can we deal with a timeout for creating a response?");
                }
            }
            catch (PostponeException)
            {
                return true;
            }
            catch (Exception e)
            {
                throw new ProgrammersErrorException($"Unexpected exception: {e}");
            }

            return true;
        }
    }
}