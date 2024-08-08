namespace AwesomeBooksApi.Models;

public class Book
{
    public Book(string isbn, string title, string author)
    {
        Isbn = NormaliseIsbn(isbn);
        Title = title;
        Author = author;
        Quantity = 0;
    }
    public string Isbn { get;  }
    public string Title { get;  }
    public string Author { get;  }
    public int Quantity { get; set; }

    public static string NormaliseIsbn(string isbn)
    {
        return isbn.Replace("-", "").Replace(" ", "");
    }

}
