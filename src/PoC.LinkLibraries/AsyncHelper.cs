using PoC.AM.Abstract.Model;

namespace PoC.LinkLibraries
{
    public static class AsyncHelper
    {
        public static void SetNextRequest(ExecutionContext context, string description)
        {
            context.NextDescription = description;
        }
    }
}