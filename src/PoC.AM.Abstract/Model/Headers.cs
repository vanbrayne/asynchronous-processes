using System;

namespace PoC.AM.Abstract.Model
{
    public class Headers
    {
        public Headers(Guid? executionId = null)
        {
            ExecutionId = executionId;
        }
        public Guid? ExecutionId { get; set; }
        public bool IsInternalCall { get; set; }
        public Guid ExecutionLockId { get; set; }
    }
}