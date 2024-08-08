using System.IO;

namespace AwesomeBooksApi.Services;
public class FileLockManager : IFileLockManager
{
    public readonly string lockDirectory = "locks";

    public FileLockManager(string lockDirectory)
    {
        lockDirectory = lockDirectory;
        Directory.CreateDirectory(lockDirectory);
    }

    public FileStream AcquireLock(string key)
    {
        var lockFilePath = Path.Combine(lockDirectory, $"{key}.lock");
        var lockFileStream = new FileStream(lockFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);

        try
        {
            lockFileStream.Lock(0, 0);
        }
        catch
        {
            lockFileStream.Dispose();
            throw new InvalidOperationException("Failed to acquire lock on the resource.");
        }

        return lockFileStream;
    }

    public void ReleaseLock(FileStream lockFileStream)
    {
        try
        {
            lockFileStream.Unlock(0, 0);
        }
        finally
        {
            lockFileStream.Dispose();
            File.Delete(lockFileStream.Name);
        }
    }
}
