using AwesomeBooksApi.Models;
using AwesomeBooksApi.Exceptions;
using AwesomeBooksApi.Repositories;
using FluentValidation;
using MediatR;

namespace AwesomeBooksApi.Books.DeleteBook;

public record DeleteBookCommand : IRequest<Unit>
{
    public DeleteBookCommand(string isbn)
    {
        Isbn = Book.NormaliseIsbn(isbn);

    }
    public string Isbn { get; }
}

public record DeleteBookResult(bool Success);

public class DeleteBookCommandValidator : AbstractValidator<DeleteBookCommand>
{
    public DeleteBookCommandValidator()
    {
        RuleFor(x => x.Isbn).NotEmpty().WithMessage("Isbn is requred.");
    }
}

public class DeleteBookHandler(IInventoryRepository repository) : IRequestHandler<DeleteBookCommand, Unit>
{
    public async Task<Unit> Handle(DeleteBookCommand command, CancellationToken cancellationToken)
    {
        var book = await repository.GetBookByIdAsync(command.Isbn);
        if (book == null)
        {
            throw new NotFoundException("Book not found.");
        }
        
        await repository.DeleteBookAsync(command.Isbn);
        return Unit.Value;
    }
}
