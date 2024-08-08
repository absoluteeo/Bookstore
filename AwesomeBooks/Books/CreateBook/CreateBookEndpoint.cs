using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace AwesomeBooks.Books.CreateBook;

public class CreateBookEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/books", async (CreateBookCommand command, ISender sender) =>
        {
            await sender.Send(command);
            return Results.Created($"/books/{command.Isbn}", new CreateBookResult(true));
        }).WithName("CreateProduct")
        .Produces<CreateBookResult>(201)
        .ProducesProblem(400)
        .WithSummary("Create a new book")
        .WithDescription("Create a new book in the inventory"); ;
    }
}
