
using AwesomeBooksApi.Data;
using AwesomeBooksApi.Exceptions;
using System.Net.Http.Json;

namespace AwesomeBooksApi.Repositories;

public class InventoryRepository(IJsonContext jsonContext) : IInventoryRepository
{
    //we do inventory repository because we are working with a json file and to add a book we need to know about all the other books (check ids etc) within a repository 

    public async Task AddBookAsync(Book book)
    {
        var inventory = await jsonContext.LoadInventoryAsync();

        inventory.Books.Add(book);
        await jsonContext.SaveInventoryAsync(inventory);
    }

    public async Task DeleteBookAsync(string isbn)
    {
        var inventory = await jsonContext.LoadInventoryAsync();
        var book = inventory.Books.FirstOrDefault(x => x.Isbn == isbn);
        //book can be null but this doesnt cause an error
        inventory.Books.Remove(book);
        await jsonContext.SaveInventoryAsync(inventory);
    }

    public async Task<IEnumerable<Book>> GetAllBooksAsync(string? searchTerm)
    {
        var inventory = await jsonContext.LoadInventoryAsync();
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return inventory.Books;
        }
        var books = inventory.GetBooksBySearchTerm(searchTerm);
        return books;
    }


    public async Task<Book?> GetBookByIdAsync(string isbn)
    {
        var inventory = await jsonContext.LoadInventoryAsync();
        var book = inventory.Books.FirstOrDefault(x => x.Isbn == isbn);
        return book;
    }

    public async Task UpdateBookAsync(Book book)
    {
        var inventory = await jsonContext.LoadInventoryAsync();
        var existingBook = inventory.Books.FirstOrDefault(x => x.Isbn == book.Isbn);
        if (existingBook == null)
        {
            throw new NotFoundException("Book not found.");
        }
        existingBook.Quantity = book.Quantity;
        await jsonContext.SaveInventoryAsync(inventory);
    }
}
