using AwesomeBooks.Data;
using Microsoft.Extensions.Options;

namespace AwesomeBooks.Seed;

public class JsonSeeder(IJsonContext jsonContext, IOptions<JsonContextSettings> options)
{
 

    public async Task SeedAsync()
    {
        if (!File.Exists(options.Value.FilePath))
        {

            Book book = new Book("978-3-16-148410-0", "The Catcher in the Rye", "James");
            var initialInventory = new Inventory
            {
                Books = new List<Book>
                    {
                        book
                    }
            };

            await jsonContext.SaveInventoryAsync(initialInventory);
        }
    }
}
