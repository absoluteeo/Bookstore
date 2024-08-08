
namespace AwesomeBooksApi.Data
{
    public interface IJsonContext
    {
        Task<Inventory> LoadInventoryAsync();
        Task SaveInventoryAsync(Inventory inventory);
    }
}