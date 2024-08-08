using AwesomeBooks;
using AwesomeBooks.Exceptions;

namespace TestProject1
{
    public class UnitTest1
    {
        [Fact]
        public void AddBook_When_IsbnExists_Throws()
        {
            var books = new List<Book>
            {
                new Book("123", "Title", "Author"),
                new Book("123", "Title", "Author")
            };

            var inventory = new Inventory();
            inventory.Books = books;

            Assert.Throws<AlreadyExistsException>(() => inventory.AddBook(new Book("123", "Title", "Author")));

        }

        [Fact]
        public void UpdateBookQuantity_When_BookNofFound_Throws()
        {
            var books = new List<Book>
            {
                new Book("123", "Title", "Author"),
      
            };

            var inventory = new Inventory();
            inventory.Books = books;

            Assert.Throws<NotFoundException>(() => inventory.UpdateBookQuantity("1234", 3));

        }

        [Fact]
        public void UpdateBookQuantity_When_QuantityLow_Throws()
        {
            var books = new List<Book>
            {
                new Book("123", "Title", "Author"),
            };

            var inventory = new Inventory();
            inventory.Books = books;

            Assert.Throws<ArgumentOutOfRangeException>(() => inventory.UpdateBookQuantity("123", -1));

        }
    }
}