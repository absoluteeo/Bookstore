using AwesomeBooks;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using AwesomeBooks.Repositories;
using AwesomeBooks.Data;
using FluentValidation.AspNetCore;
using MediatR;
using AwesomeBooks.Books.CreateBook;
using AwesomeBooks.Seed;
using Microsoft.Extensions.Hosting;
using AwesomeBooks.Books.GetBookByIsbn;
using AwesomeBooks.Books.GetBooks;
using AwesomeBooks.Books.DeleteBook;

var inventory = new Inventory();
var book1 = new Book("978-3-16-148410-0", "The Catcher in the Rye", "J.D. Salinger");
var book2 = new Book("978-3-16-148410-2", "The Catcher in the Rye", "J.D. Salinger");

inventory.AddBook(book1);
inventory.AddBook(book2);



var searchResult = inventory.GetBooksBySearchTerm("Salinger");

foreach (var book in searchResult)
{
    Console.WriteLine($"Title: {book.Title}, Author: {book.Author}");
}

var assembly = typeof(Program).Assembly;


var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.Configure<JsonContextSettings>(options =>
        {
            options.FilePath = "persistence/inventory.json"; 
        });
        services.AddScoped<IJsonContext, JsonContext>();
        services.AddTransient<JsonSeeder>();
        services.AddScoped<IInventoryRepository, InventoryRepository>();
        services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(Program).Assembly));
        services.AddValidatorsFromAssembly(typeof(Program).Assembly);
    })
    .Build();


using (var scope = host.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<JsonSeeder>();
    await seeder.SeedAsync();

    var sender = scope.ServiceProvider.GetRequiredService<ISender>();
    var createBookCommand = new CreateBookCommand("978-3-16-148410-31", "The Catcher in the Rye", "J.D. Salinger");
    var createBookCommand2 = new CreateBookCommand("978-3-16-148410-42", "The Catcher in the Rye 2", "J.D. Salinger");
    var createBookCommand3 = new CreateBookCommand("978-3-16-148410-53", "The Catcher in the Rye 3", "J.D. Salinger");

    await sender.Send(createBookCommand);
    await sender.Send(createBookCommand2);
    await sender.Send(createBookCommand3);

    Console.WriteLine("Books added");

    var getBookQuery = new GetBookByIsbnQuery("978-3-16-148410-3");

    var result = await sender.Send(getBookQuery);
    Console.WriteLine($"Title: {result.book.Title}, Author: {result.book.Author}, ISBN: {result.book.Isbn}");

    var getBooksQuery = new GetBooksQuery("Salinger");
    var results = await sender.Send(getBooksQuery);

    var deleteCommand = new DeleteBookCommand("978-3-16-148410-3");
    await sender.Send(deleteCommand);
    Console.WriteLine("Sent delete command");
    foreach (var item in searchResult)
    {
        Console.WriteLine($"Title: {item.Title}, Author: {item.Author}, ISBN: {item.Isbn}");
    }

}

await host.RunAsync();


Console.WriteLine("Book added");