using ReenbitTest.Server.Models;

namespace ReenbitTest.Server.Interfaces
{
    public interface I_ItemRepository
    {
        Task<List<string>> GetAllItemsAsync();
        Task AddItemAsync(Item item);
        Task<Stream> GetItemByNameAsync(string name);
        Task DeleteItemAsync(string name);

    }
}
