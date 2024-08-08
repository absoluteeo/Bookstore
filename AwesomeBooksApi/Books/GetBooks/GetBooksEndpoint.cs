using Carter;
using MediatR;

namespace AwesomeBooksApi.Books.GetBooks;

public class GetBooksEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/books", async ([AsParameters] GetBooksQuery query, ISender sender) =>
        {
            var result = await sender.Send(query);
            return result;
        }).Produces<GetBooksResult>(200)
        .ProducesProblem(400)
        .WithSummary("Get all books")
        .WithDescription("Get all books from the inventory optionally with a matching search term");
    }
}

