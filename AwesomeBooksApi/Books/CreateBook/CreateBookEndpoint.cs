using Carter;
using MediatR;

namespace AwesomeBooksApi.Books.CreateBook;

public class CreateBookEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/books", async (CreateBookCommand command, ISender sender) =>
        {
            await sender.Send(command);
            return Results.Created($"/books/{command.Isbn}", new Unit());
        }).WithName("CreateBook")
        .Produces(201)
        .ProducesProblem(400)
        .WithSummary("Create a new book")
        .WithDescription("Create a new book in the inventory"); ;
    }
}
