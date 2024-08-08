using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Xml;

namespace AwesomeBooksApi.Data;

public class JsonContext(IOptions<JsonContextSettings> options) : IJsonContext
{
   
    // Load the inventory from the JSON file
    public async Task<Inventory> LoadInventoryAsync()
    {
        var filePath = options.Value.FilePath;
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("Inventory file not found", filePath);
        }

        using (var reader = new StreamReader(filePath))
        {
            var json = await reader.ReadToEndAsync();
            return JsonConvert.DeserializeObject<Inventory>(json);
        }
    }

    public async Task SaveInventoryAsync(Inventory inventory)
    {
        var filePath = options.Value.FilePath;

        var directory = Path.GetDirectoryName(filePath);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }


        var json = JsonConvert.SerializeObject(inventory, Newtonsoft.Json.Formatting.Indented);

        using (var writer = new StreamWriter(filePath))
        {
            await writer.WriteAsync(json);
        }
    }
}
