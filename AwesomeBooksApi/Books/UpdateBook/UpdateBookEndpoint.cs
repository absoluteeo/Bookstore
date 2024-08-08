using Carter;
using MediatR;

namespace AwesomeBooksApi.Books.UpdateBook;

public class UpdateBookEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/books", async (UpdateBookCommand command, ISender sender) =>
        {
            await sender.Send(command);
            return Results.Ok();
        }).WithName("Update boon")
        .Produces(200)
        .ProducesProblem(400)
        .ProducesProblem(404)
        .WithSummary("Update a book")
        .WithDescription("Update a book in the inventory"); ;
    }
}
