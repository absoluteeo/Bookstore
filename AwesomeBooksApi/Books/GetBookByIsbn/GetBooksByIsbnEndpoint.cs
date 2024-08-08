using Carter;
using MediatR;

namespace AwesomeBooksApi.Books.GetBookByIsbn;

public class GetBooksByIsbnEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/books/{isbn}", async (string isbn, ISender sender) =>
        {
            var query = new GetBookByIsbnQuery(isbn);
            var result = await sender.Send(query);
            return result;
        }).Produces<Book>(200)
        .ProducesProblem(404)
        .WithSummary("Get a book by ISBN")
        .WithDescription("Get a book from the inventory by its ISBN");
    }
}

