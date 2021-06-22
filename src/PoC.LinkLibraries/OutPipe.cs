using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PoC.AM.Abstract;
using PoC.AM.Abstract.Exceptions;
using PoC.AM.Abstract.Model;

namespace PoC.LinkLibraries
{
    public class OutPipe
    {
        private readonly IAsyncManagement _asyncManager;

        public OutPipe(IAsyncManagement asyncManager)
        {
            _asyncManager = asyncManager;
        }

        public async Task<T> SendRequestAsync<T>(RemoteMethodDelegate remoteMethod, ExecutionContext context, int subRequestNumber, string description)
        {
            if (TryGetSavedResult(context, subRequestNumber, description, out var subRequest, out T result)) return result;

            var requestId = await _asyncManager.CreateAsyncRequestAsync(subRequest.Description, context.ExecutionId, remoteMethod);
            subRequest.RequestId = requestId;
            await _asyncManager.AddSubRequestAsync(context.ExecutionId, subRequest.Identifier, subRequest);
            throw new PostponeException();
        }

        public async Task<T> MakeCallIdempotent<T>(ExecutionContext context, int subRequestNumber, string description, Func<Task<T>> method)
        {
            if (TryGetSavedResult(context, subRequestNumber, description, out var subRequest, out T result)) return result;

            subRequest.HasCompleted = true;
            try
            {
                result = await method();
                subRequest.ResultValueAsJson = result.ToJsonString();
                if (context.IsAsynchronous)
                {
                    await _asyncManager.AddSubRequestAsync(context.ExecutionId, subRequest.Identifier, subRequest);
                }
            }
            catch (Exception e)
            {
                subRequest.ExceptionTypeName = e.GetType().FullName;
                throw new SubRequestException(subRequest);
            }

            return result;
        }

        public async Task MakeCallIdempotent(ExecutionContext context, int subRequestNumber, string description, Func<Task> method)
        {
            if (TryGetSavedResult(context, subRequestNumber, description, out var subRequest, out dynamic _)) return;
            
            subRequest.HasCompleted = true;
            try
            {
                await method();
                if (context.IsAsynchronous)
                {
                    await _asyncManager.AddSubRequestAsync(context.ExecutionId, subRequest.Identifier, subRequest);
                }
            }
            catch (Exception e)
            {
                subRequest.ExceptionTypeName = e.GetType().FullName;
                throw new SubRequestException(subRequest);
            }
        }

        private static bool TryGetSavedResult<T>(ExecutionContext context, int subRequestNumber, string description,
            out SubRequest subRequest, out T result)
        {
            result = default;
            lock (context.SubRequests)
            {
                var identifier = $"{context.ExecutionId}-{subRequestNumber}";
                if (context.SubRequests.ContainsKey(identifier))
                {
                    subRequest = context.SubRequests[identifier];
                    if (subRequest.Description != description) throw new ProgrammersErrorException($"Expected description \"{subRequest.Description}\" for SubRequest {subRequestNumber}, but received \"{description}\".");
                    if (!subRequest.HasCompleted) throw new PostponeException();
                    if (subRequest.HasFailed) throw new SubRequestException(subRequest);
                    result = subRequest.ResultValueAsJson == null ? default : JsonConvert.DeserializeObject<T>(subRequest.ResultValueAsJson);
                    return true;
                }

                subRequest = new SubRequest(identifier)
                {
                    Description = description
                };
                // TODO Double?
                // context.SubRequests.Add(identifier, subRequest);
            }

            return false;
        }
    }
}