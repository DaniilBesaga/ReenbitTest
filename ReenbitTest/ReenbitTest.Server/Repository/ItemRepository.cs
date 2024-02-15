using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;
using ReenbitTest.Server.Interfaces;
using ReenbitTest.Server.Models;
using System.Reflection.Metadata;

namespace ReenbitTest.Server.Repository
{
    public class ItemRepository : I_ItemRepository
    {
        private readonly BlobServiceClient _blobServiceClient;

        public ItemRepository(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
        }
        public async Task AddItemAsync(Item item)
        {
            var containerInstance = _blobServiceClient.GetBlobContainerClient("test");
            var blobInstance = containerInstance.GetBlobClient(item.File.FileName);
            IDictionary<string, string> metadata =
            new Dictionary<string, string>();

            metadata["email"] = $"{item.Email}";

            await containerInstance.SetMetadataAsync(metadata);
            await blobInstance.UploadAsync(item.File.OpenReadStream(), metadata: metadata);
        }

        public async Task DeleteItemAsync(string name)
        {
            var containerInstance = _blobServiceClient.GetBlobContainerClient("test");
            var blobInstance = containerInstance.GetBlobClient(name);
            await blobInstance.DeleteAsync();
        }

        public async Task<List<string>> GetAllItemsAsync()
        {
            var containerInstance = _blobServiceClient.GetBlobContainerClient("test");
            var items = new List<string>();

            await foreach (var blobItem in containerInstance.GetBlobsAsync())
            {
                items.Add(blobItem.Name);
            }

            return items;
        }

        public async Task<Stream> GetItemByNameAsync(string name)
        {
            var containerInstance = _blobServiceClient.GetBlobContainerClient("test");
            var blobInstance = containerInstance.GetBlobClient(name);
            var downloadContent = await blobInstance.DownloadAsync();
            return downloadContent.Value.Content;
        }
    }
}
