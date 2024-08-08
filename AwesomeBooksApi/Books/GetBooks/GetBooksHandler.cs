using AwesomeBooksApi.Repositories;
using MediatR;

namespace AwesomeBooksApi.Books.GetBooks;

public record GetBooksQuery(string? searchTerm) : IRequest<GetBooksResult>;

public record GetBooksResult(IEnumerable<Book> Books);
internal class GetBooksHandler(IInventoryRepository repository) : IRequestHandler<GetBooksQuery, GetBooksResult>
{
    public async Task<GetBooksResult> Handle(GetBooksQuery query, CancellationToken cancellationToken)
    {
        var books = await repository.GetAllBooksAsync(query.searchTerm);
        return new GetBooksResult(books);
    }
}
