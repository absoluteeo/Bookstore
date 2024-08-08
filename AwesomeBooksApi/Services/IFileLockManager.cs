
namespace AwesomeBooksApi.Services
{
    public interface IFileLockManager
    {
        FileStream AcquireLock(string key);
        void ReleaseLock(FileStream lockFileStream);
    }
}