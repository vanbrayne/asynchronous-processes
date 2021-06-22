namespace PoC.AM.Abstract.Model
{
    public interface ILockable
    {
        public LockWithTimeout LockWithTimeout { get; }
    }
}