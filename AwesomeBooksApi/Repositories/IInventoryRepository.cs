namespace AwesomeBooksApi.Repositories;

internal interface IInventoryRepository
{
    Task AddBookAsync(Book book);
    Task DeleteBookAsync(string isbn);
    Task<Book?> GetBookByIdAsync(string isbn);
    Task<IEnumerable<Book>> GetAllBooksAsync(string? searchTerm);
    Task UpdateBookAsync(Book book);
}
