using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using FluentValidation.TestHelper;
using AwesomeBooksApi.Books.DeleteBook;
using AwesomeBooksApi.Exceptions;
using AwesomeBooksApi.Repositories;
using AwesomeBooksApi.Models;
using MediatR;

namespace AwesomeBooksApi.Books.DeleteBook.Tests;
public class DeleteBookHandlerTests
{
    private readonly DeleteBookCommandValidator _validator;
    private readonly Mock<IInventoryRepository> _repositoryMock;
    private readonly DeleteBookHandler _handler;

    public DeleteBookHandlerTests()
    {
        _validator = new DeleteBookCommandValidator();
        _repositoryMock = new Mock<IInventoryRepository>();
        _handler = new DeleteBookHandler(_repositoryMock.Object);
    }

    [Fact]
    public void Validator_Should_Have_Error_When_Isbn_Is_Empty()
    {
        var command = new DeleteBookCommand("");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Isbn)
              .WithErrorMessage("Isbn is requred.");
    }

    [Fact]
    public async Task Handler_Should_Delete_Book_When_It_Exists()
    {
        // Arrange
        var command = new DeleteBookCommand("1234567890");
        var existingBook = new Book(command.Isbn, "Existing Title", "Existing Author");
        _repositoryMock.Setup(repo => repo.GetBookByIdAsync(command.Isbn))
                       .ReturnsAsync(existingBook);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(repo => repo.DeleteBookAsync(command.Isbn), Times.Once);
        Xunit.Assert.Equal(Unit.Value, result);
    }

    [Fact]
    public async Task Handler_Should_Throw_Exception_When_Book_Does_Not_Exist()
    {
        // Arrange
        var command = new DeleteBookCommand("1234567890");
        _repositoryMock.Setup(repo => repo.GetBookByIdAsync(command.Isbn))
                       .ReturnsAsync((Book)null);

        // Act & Assert
        await Xunit.Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));
    }
}