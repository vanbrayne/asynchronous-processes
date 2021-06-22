using System;
using System.Threading.Tasks;
using PoC.AM.Abstract.Model;

namespace PoC.LinkLibraries
{
    public class RestClient
    {
        private readonly OutPipe _outPipe;

        public RestClient(OutPipe outPipe)
        {
            _outPipe = outPipe;
        }

        public async Task<T> SendRequestAsync<T>(RemoteMethodDelegate remoteMethod, ExecutionContext context)
        { 
            if (!context.IsAsynchronous)
            {
                return (await MakeCallIdempotent(context, () => remoteMethod(new Headers()))).ToObjectFromJson<T>();
            }
            return await _outPipe.SendRequestAsync<T>(remoteMethod, context, context.NextSubRequestNumber++, context.NextDescription);
        }

        public Task<T> MakeCallIdempotent<T>(ExecutionContext context, Func<Task<T>> remoteMethod)
        {
            return _outPipe.MakeCallIdempotent(context, context.NextSubRequestNumber++, context.NextDescription, remoteMethod);
        }

        public Task MakeCallIdempotent(ExecutionContext context, Func<Task> remoteMethod)
        {
            return _outPipe.MakeCallIdempotent(context, context.NextSubRequestNumber++, context.NextDescription, remoteMethod);
        }
    }
}