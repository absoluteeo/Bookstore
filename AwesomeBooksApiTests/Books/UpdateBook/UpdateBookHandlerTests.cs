using Xunit;
using Moq;
using FluentValidation.TestHelper;
using AwesomeBooksApi.Books.UpdateBook;
using AwesomeBooksApi.Exceptions;
using AwesomeBooksApi.Repositories;
using AwesomeBooksApi.Models;
using AwesomeBooksApi.Services;
using MediatR;

namespace AwesomeBooksApi.Books.UpdateBook.Tests;


[CollectionDefinition("SequentialTests", DisableParallelization = true)]
public class SequentialTestsCollection
{

}

[Collection("SequentialTests")]
public class UpdateBookCommandTests
{
    private readonly UpdateBookCommandValidator _validator;
    private readonly Mock<IInventoryRepository> _repositoryMock;
    private readonly Mock<IFileLockManager> _lockManagerMock;
    private readonly UpdateBookHandler _handler;

    public UpdateBookCommandTests()
    {
        _validator = new UpdateBookCommandValidator();
        _repositoryMock = new Mock<IInventoryRepository>();
        _lockManagerMock = new Mock<IFileLockManager>();
        _handler = new UpdateBookHandler(_repositoryMock.Object, _lockManagerMock.Object);
    }

    [Fact]
    public void Validator_Should_Have_Error_When_Isbn_Is_Empty()
    {
        var command = new UpdateBookCommand("", 10);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Isbn)
              .WithErrorMessage("Isbn is requred.");
    }

    [Fact]
    public void Validator_Should_Have_Error_When_Quantity_Is_Negative()
    {
        var command = new UpdateBookCommand("1234567890", -5);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Quantity)
              .WithErrorMessage("Negative quanitities not allowed");
    }

    [Fact]
    public async Task Handler_Should_Update_Book_When_It_Exists()
    {
        // Arrange
        var command = new UpdateBookCommand("1234567890", 10);
        var existingBook = new Book(command.Isbn, "Title", "Author") { Quantity = 5 };

        _repositoryMock.Setup(repo => repo.GetBookByIdAsync(command.Isbn))
                       .ReturnsAsync(existingBook);

        var mockLockFileStream = new Mock<FileStream>("test1", FileMode.OpenOrCreate, FileAccess.ReadWrite);

        _lockManagerMock.Setup(lm => lm.AcquireLock(command.Isbn))
                        .Returns(mockLockFileStream.Object);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(repo => repo.UpdateBookAsync(It.Is<Book>(
            b => b.Isbn == command.Isbn &&
                 b.Quantity == command.Quantity)), Times.Once);

        _lockManagerMock.Verify(lm => lm.AcquireLock(command.Isbn), Times.Once);
        _lockManagerMock.Verify(lm => lm.ReleaseLock(It.IsAny<FileStream>()), Times.Once);
        Xunit.Assert.Equal(Unit.Value, result);
    }

    [Fact]
    public async Task Handler_Should_Throw_Exception_When_Book_Does_Not_Exist()
    {
        // Arrange
        var command = new UpdateBookCommand("1234567890", 10);
        _repositoryMock.Setup(repo => repo.GetBookByIdAsync(command.Isbn))
                       .ReturnsAsync((Book)null);
        _lockManagerMock.Setup(lm => lm.AcquireLock(command.Isbn))
                        .Returns(new FileStream("test2", FileMode.OpenOrCreate));

        // Act & Assert
        await Xunit.Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));

        _lockManagerMock.Verify(lm => lm.ReleaseLock(It.IsAny<FileStream>()), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Release_Lock_When_Exception_Occurs()
    {
        // Arrange
        var command = new UpdateBookCommand("1234567890", 10);
        var mockLockFileStream = new Mock<FileStream>("test3", FileMode.OpenOrCreate, FileAccess.ReadWrite);

        _repositoryMock.Setup(repo => repo.GetBookByIdAsync(command.Isbn))
                       .ThrowsAsync(new System.Exception("Database error"));
        _lockManagerMock.Setup(lm => lm.AcquireLock(command.Isbn))
                        .Returns(mockLockFileStream.Object);

        // Act & Assert
        await Xunit.Assert.ThrowsAsync<System.Exception>(() => _handler.Handle(command, CancellationToken.None));

        _lockManagerMock.Verify(lm => lm.AcquireLock(command.Isbn), Times.Once);
        _lockManagerMock.Verify(lm => lm.ReleaseLock(It.IsAny<FileStream>()), Times.Once);
    }
}