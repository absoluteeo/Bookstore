
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using AwesomeBooks.Repositories;
using AwesomeBooks.Data;
using MediatR;
using AwesomeBooks.Books.CreateBook;
using AwesomeBooks.Seed;
using Microsoft.Extensions.Hosting;
using AwesomeBooks.Books.GetBookByIsbn;
using AwesomeBooks.Books.GetBooks;
using AwesomeBooks.Books.DeleteBook;
using Microsoft.AspNetCore.Builder;
using Carter;




var builder = WebApplication.CreateBuilder(args);

var assembly = typeof(Program).Assembly;




builder.Services.Configure<JsonContextSettings>(options =>
{
    options.FilePath = "persistence/inventory.json"; 
});
builder.Services.AddScoped<IJsonContext, JsonContext>();
builder.Services.AddTransient<JsonSeeder>();
builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
builder.Services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(Program).Assembly));
builder.Services.AddCarter();
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<JsonSeeder>();
    await seeder.SeedAsync();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.MapCarter();

app.Run();

//using (var scope = host.Services.CreateScope())
//{
    

//    var sender = scope.ServiceProvider.GetRequiredService<ISender>();
//    var createBookCommand = new CreateBookCommand("978-3-16-148410-31", "The Catcher in the Rye", "J.D. Salinger");
//    var createBookCommand2 = new CreateBookCommand("978-3-16-148410-42", "The Catcher in the Rye 2", "J.D. Salinger");
//    var createBookCommand3 = new CreateBookCommand("978-3-16-148410-53", "The Catcher in the Rye 3", "J.D. Salinger");

//    await sender.Send(createBookCommand);
//    await sender.Send(createBookCommand2);
//    await sender.Send(createBookCommand3);

//    Console.WriteLine("Books added");

//    var getBookQuery = new GetBookByIsbnQuery("978-3-16-148410-3");

//    var result = await sender.Send(getBookQuery);
//    Console.WriteLine($"Title: {result.book.Title}, Author: {result.book.Author}, ISBN: {result.book.Isbn}");

//    var getBooksQuery = new GetBooksQuery("Salinger");
//    var results = await sender.Send(getBooksQuery);

//    var deleteCommand = new DeleteBookCommand("978-3-16-148410-3");
//    await sender.Send(deleteCommand);
//    Console.WriteLine("Sent delete command");
//    foreach (var item in searchResult)
//    {
//        Console.WriteLine($"Title: {item.Title}, Author: {item.Author}, ISBN: {item.Isbn}");
//    }

//}

//await host.RunAsync();



// TODO: Input validation 
// Endpoints or CLI
