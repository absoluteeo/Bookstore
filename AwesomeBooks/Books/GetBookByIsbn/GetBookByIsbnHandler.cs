using AwesomeBooks.Exceptions;
using AwesomeBooks.Repositories;
using MediatR;

namespace AwesomeBooks.Books.GetBookByIsbn;

public record GetBookByIsbnQuery : IRequest<GetBookResult>
{
    public GetBookByIsbnQuery(string isbn)
    {
        Isbn = Book.NormaliseIsbn(isbn);

    }
    public string Isbn { get; }
}
public record GetBookResult(Book book);
internal class GetBookByIsbnHandler(IInventoryRepository repository) : IRequestHandler<GetBookByIsbnQuery, GetBookResult>
{
    public async Task<GetBookResult> Handle(GetBookByIsbnQuery query, CancellationToken cancellationToken)
    {
        var isbn = Book.NormaliseIsbn(query.Isbn);
        var book = await repository.GetBookByIdAsync(isbn);
        if (book == null)
        {
            throw new NotFoundException("Book", isbn);
        }

        return new GetBookResult(book);
    }
}
