using AwesomeBooksApi.Exceptions;
using AwesomeBooksApi.Repositories;
using AwesomeBooksApi.Services;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AwesomeBooksApi.Books.UpdateBook;

public record UpdateBookCommand : IRequest<Unit>
{
    public UpdateBookCommand(string isbn, int quantity)
    {
        Isbn = Book.NormaliseIsbn(isbn);
        Quantity = quantity;
    }

    public string Isbn { get; }

    public int Quantity { get; }
}

public class UpdateBookCommandValidator : AbstractValidator<UpdateBookCommand>
{
    public UpdateBookCommandValidator()
    {
        RuleFor(x => x.Isbn)
            .NotEmpty()
            .WithMessage("Isbn is requred.");
        RuleFor(x => x.Quantity)
            .NotEmpty()
            .WithMessage("Quantity is required")
            .GreaterThanOrEqualTo(0)
            .WithMessage("Negative quanitities not allowed");
    }
}

internal class UpdateBookHandler(IInventoryRepository repository, FileLockManager lockManager) : IRequestHandler<UpdateBookCommand, Unit>
{
    public async Task<Unit> Handle(UpdateBookCommand command, CancellationToken cancellationToken)
    {


        FileStream lockFileStream = null;
        try
        {
            lockFileStream = lockManager.AcquireLock(command.Isbn);

            //improvement is add a count or check method on repository to check if book exists

            var existingBook = await repository.GetBookByIdAsync(command.Isbn);
            if (existingBook is null)
            {
                throw new NotFoundException("Book", command.Isbn);
            }

            existingBook.Quantity = command.Quantity;
            await repository.UpdateBookAsync(existingBook);
        }
        finally
        {
            if (lockFileStream != null)
            {
                lockManager.ReleaseLock(lockFileStream);
            }
        }
        
        
        return Unit.Value;
    }
}
