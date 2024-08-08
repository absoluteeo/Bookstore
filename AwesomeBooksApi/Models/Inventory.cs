namespace AwesomeBooksApi.Models;

public class Inventory
{
    public List<Book> Books { get; set; } = new();

    private bool BookExists(string isbn)
    {
        return Books.Any(b => b.Isbn == Book.NormaliseIsbn(isbn));
    }

    public void AddBook(Book book)
    {
        var exists = BookExists(book.Isbn);
        if (exists) {
            throw new AlreadyExistsException("Book", book.Isbn);
        }
        Books.Add(book);
    }

    public void RemoveBook(string isbn)
    {
        var book = Books.Find(b => b.Isbn == isbn);
        if (book is null)
        {
            throw new NotFoundException("Book", isbn);
        }
        Books.Remove(book);
    }

    public IEnumerable<Book> GetBooksBySearchTerm(string searchTerm) 
    {
        var books = Books.Where(b => b.Author.Contains(searchTerm) || b.Title.Contains(searchTerm));
        return books;
    }
}




