using AwesomeBooksApi.Data;
using AwesomeBooksApi.Repositories;
using AwesomeBooksApi.Seed;
using AwesomeBooksApi.Services;
using Carter;
using FluentValidation;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

var assembly = typeof(Program).Assembly;


builder.Services.Configure<JsonContextSettings>(options =>
{
    options.FilePath = "persistence/inventory.json";
    
});
builder.Services.Configure<FileLockOptions>(options =>
{
    options.LockDirectory = "locks";
});
builder.Services.AddScoped<IJsonContext, JsonContext>();
builder.Services.AddTransient<JsonSeeder>();
builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
builder.Services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(Program).Assembly));
builder.Services.AddCarter();
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IFileLockManager, FileLockManager>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<JsonSeeder>();
    await seeder.SeedAsync();
}


app.UseSwagger();
app.UseSwaggerUI();


app.MapCarter();
app.UseSwagger();
app.UseSwaggerUI();
app.Run();
