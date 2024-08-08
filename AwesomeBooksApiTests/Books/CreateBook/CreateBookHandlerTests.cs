using Xunit;
using AwesomeBooksApi.Books.CreateBook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using AwesomeBooksApi.Repositories;
using FluentValidation.TestHelper;
using AwesomeBooksApi.Models;
using MediatR;
using AwesomeBooksApi.Exceptions;

namespace AwesomeBooksApi.Books.CreateBook.Tests
{
    public class CreateBookHandlerTests
    {
        private readonly CreateBookCommandValidator _validator;
        private readonly Mock<IInventoryRepository> _repositoryMock;
        private readonly CreateBookHandler _handler;

        public CreateBookHandlerTests()
        {
            _validator = new CreateBookCommandValidator();
            _repositoryMock = new Mock<IInventoryRepository>();
            _handler = new CreateBookHandler(_repositoryMock.Object);
        }

        [Fact]
        public void Validator_Should_Have_Error_When_Isbn_Is_Empty()
        {
            var command = new CreateBookCommand("", "Title", "Author");
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Isbn);
        }

        [Fact]
        public void Validator_Should_Have_Error_When_Title_Is_Empty()
        {
            var command = new CreateBookCommand("1234567890", "", "Author");
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Title);
        }

        [Fact]
        public void Validator_Should_Have_Error_When_Title_Length_Is_Invalid()
        {
            var command = new CreateBookCommand("1234567890", "A", "Author");
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Title)
                  .WithErrorMessage("Bad name");
        }

        [Fact]
        public void Validator_Should_Have_Error_When_Author_Is_Empty()
        {
            var command = new CreateBookCommand("1234567890", "Title", "");
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Author);
        }

        [Fact]
        public async Task Handler_Should_Add_Book_When_It_Does_Not_Exist()
        {
            // Arrange
            var command = new CreateBookCommand("1234567890", "Title", "Author");
            _repositoryMock.Setup(repo => repo.GetBookByIdAsync(command.Isbn))
                           .ReturnsAsync((Book)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(repo => repo.AddBookAsync(It.Is<Book>(
                b => b.Isbn == command.Isbn &&
                     b.Title == command.Title &&
                     b.Author == command.Author)), Times.Once);

            Xunit.Assert.Equal(Unit.Value, result);
        }

        [Fact]
        public async Task Handler_Should_Throw_Exception_When_Book_Already_Exists()
        {
            // Arrange
            var command = new CreateBookCommand("1234567890", "Title", "Author");
            var existingBook = new Book(command.Isbn, "Existing Title", "Existing Author");
            _repositoryMock.Setup(repo => repo.GetBookByIdAsync(command.Isbn))
                           .ReturnsAsync(existingBook);

            // Act & Assert
            await Xunit.Assert.ThrowsAsync<AlreadyExistsException>(() => _handler.Handle(command, CancellationToken.None));
        }
    }
}