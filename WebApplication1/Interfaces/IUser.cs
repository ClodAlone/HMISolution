namespace Interfaces
{
    public interface IUser
    {
        public long GetReadableAccessMask();
        public long GetWritableAccessMask();
        public long GetAccessLevel();
    }
}