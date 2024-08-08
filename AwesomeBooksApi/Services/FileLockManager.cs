using Microsoft.Extensions.Options;
using System.IO;

namespace AwesomeBooksApi.Services;
public class FileLockManager : IFileLockManager
{
    public readonly string _lockDirectory = "locks";

    public FileLockManager(IOptions<FileLockOptions> options)
    {
        _lockDirectory = options.Value.LockDirectory;
        Directory.CreateDirectory(_lockDirectory);
    }

    public FileStream AcquireLock(string key)
    {
        var lockFilePath = Path.Combine(_lockDirectory, $"{key}.lock");

        FileStream lockFileStream = null;
        try
        {
            lockFileStream = new FileStream(lockFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
            lockFileStream.Lock(0, 0);
        }
        catch (IOException ex)
        {
            lockFileStream?.Dispose();
            Console.WriteLine($"IOException: {ex.Message}"); // Debugging output
            throw new InvalidOperationException("Failed to acquire lock on the resource.", ex);
        }
        catch (Exception ex)
        {
            lockFileStream?.Dispose();
            Console.WriteLine($"Exception: {ex.Message}"); // Debugging output
            throw new InvalidOperationException("Failed to acquire lock on the resource.", ex);
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

            if (File.Exists(lockFileStream.Name))
            {
                File.Delete(lockFileStream.Name);
            }
        }
    }
}
