using AwesomeBooks.Exceptions;
using AwesomeBooks.Repositories;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AwesomeBooks.Books.CreateBook;

public record CreateBookCommand : IRequest<Unit>
{
    public CreateBookCommand(string isbn, string title, string author)
    {
        Isbn = Book.NormaliseIsbn(isbn);
        Title = title;
        Author = author;
    }

    public string Isbn { get; }
    public string Title { get; }
    public string Author { get; }

}
public record CreateBookResult(bool Success);

public class CreateBookCommandValidator : AbstractValidator<CreateBookCommand>
{
    public CreateBookCommandValidator()
    {
        RuleFor(x => x.Isbn).NotEmpty().WithMessage("Isbn is requred.");
        RuleFor(x => x.Title).NotEmpty().WithMessage("Title is required")
                .Length(2, 100).WithMessage("Bad name");
        RuleFor(x => x.Author).NotEmpty();
    }
}

internal class CreateBookHandler(IInventoryRepository repository) : IRequestHandler<CreateBookCommand, Unit>
{
    public async Task<Unit> Handle(CreateBookCommand command, CancellationToken cancellationToken)
    {

        //improvement is add a count or check method on repository to check if book exists
        var existingBook = await repository.GetBookByIdAsync(command.Isbn);
        if (existingBook is not null)
        {
            throw new AlreadyExistsException("Book", command.Isbn);
        }

        var book = new Book(command.Isbn, command.Title, command.Author);
        await repository.AddBookAsync(book);
        return Unit.Value;
    }
}
