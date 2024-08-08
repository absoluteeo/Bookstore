using Carter;
using MediatR;

namespace AwesomeBooksApi.Books.DeleteBook;

public class DeleteBookEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/books/{isbn}", async (string isbn, ISender sender) =>
        {
            var command = new DeleteBookCommand(isbn);
            await sender.Send(command);
            return Results.NoContent();
        }).WithName("DeleteBook")
         .Produces(200)
        .ProducesProblem(400)
        .ProducesProblem(404)
        .WithSummary("Delete a book")
        .WithDescription("Delete a book in the inventory"); ;
    }
}
