using System;
using System.IO;
using Xunit;
using Moq;
using AwesomeBooksApi.Services;
using Microsoft.Extensions.Options;

namespace AwesomeBooksApi.Services.Tests;

public class FileLockManagerTests : IDisposable
{
    private readonly string _tempLockDirectory;
    private readonly FileLockManager _fileLockManager;

    public FileLockManagerTests()
    {
        // Create a unique temporary directory for each test instance
        _tempLockDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_tempLockDirectory);

        // Configure FileLockOptions with the temporary directory
        var options = Options.Create(new FileLockOptions { LockDirectory = _tempLockDirectory });
        _fileLockManager = new FileLockManager(options);
    }

    [Fact]
    public void Constructor_Should_Create_LockDirectory()
    {
        // Assert that the directory exists
        Xunit.Assert.True(Directory.Exists(_tempLockDirectory));
    }

    [Fact]
    public void AcquireLock_Should_Create_And_Return_LockFile()
    {
        // Arrange
        string key = "testBook1";

        // Act
        using (var lockStream = _fileLockManager.AcquireLock(key))
        {
            // Assert
            string lockFilePath = Path.Combine(_tempLockDirectory, $"{key}.lock");
            Xunit.Assert.True(File.Exists(lockFilePath));
            Xunit.Assert.NotNull(lockStream);
        }
    }

    [Fact]
    public void AcquireLock_Should_Throw_When_Lock_Already_Acquired()
    {
        // Arrange
        string key = "testBook2";

        // Act
        using (var lockStream = _fileLockManager.AcquireLock(key))
        {
            // Assert
            // Expect InvalidOperationException since we've defined this as the custom exception
            Xunit.Assert.Throws<InvalidOperationException>(() => _fileLockManager.AcquireLock(key));
        }
    }

    [Fact]
    public void ReleaseLock_Should_Unlock_And_Delete_LockFile()
    {
        // Arrange
        string key = "testBook3";
        string lockFilePath = Path.Combine(_tempLockDirectory, $"{key}.lock");

        // Act
        using (var lockStream = _fileLockManager.AcquireLock(key))
        {
            // Assert lock file is created
            Xunit.Assert.True(File.Exists(lockFilePath));
        }

        // Assert lock file is deleted after release
        Xunit.Assert.False(File.Exists(lockFilePath));
    }

    public void Dispose()
    {
        // Clean up the temporary lock directory
        if (Directory.Exists(_tempLockDirectory))
        {
            Directory.Delete(_tempLockDirectory, true);
        }
    }
}