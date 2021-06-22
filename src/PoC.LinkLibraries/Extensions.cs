using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PoC.AM.Abstract.Model;

namespace PoC.LinkLibraries
{
    public static class Extensions
    {
        public static T AsCopy<T>(this T source) where T : class
        {
            var json = JsonConvert.SerializeObject(source);
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static string ToJsonString<T>(this T source)
        {
            return JsonConvert.SerializeObject(source);
        }

        public static T ToObjectFromJson<T>(this string source)
        {
            if (source == null) return default;
            return JsonConvert.DeserializeObject<T>(source);
        }

        public static Task<Guid> WaitForLockAsync(this ILockable lockable, TimeSpan? lockTimeSpan = null, TimeSpan? maxWaitTimeSpan = null)
        {
            return lockable.LockWithTimeout.WaitForLockAsync(lockTimeSpan, maxWaitTimeSpan);
        }

        public static void Unlock(this ILockable lockable, Guid lockId)
        {
            lockable.LockWithTimeout.Unlock(lockId);
        }
    }
}
