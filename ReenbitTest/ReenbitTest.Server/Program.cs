using Azure.Storage.Blobs;
using ReenbitTest.Server.Interfaces;
using ReenbitTest.Server.Repository;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
// Add services to the container.

builder.Services.AddScoped(_ =>
{
    return new BlobServiceClient(builder.Configuration.GetConnectionString("AzureStorageBlob"));
});
builder.Services.AddScoped<I_ItemRepository, ItemRepository>();
var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
