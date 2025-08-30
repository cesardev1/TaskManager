using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using TaskManager.Models;

namespace TaskManager.Services;

public class AzureFilesRepository : IFileStore
{
    private string _connectionString;
    public AzureFilesRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("AzureStorageConnectionString");
    }

    public async Task Delete(string container, string path)
    {
        if (string.IsNullOrEmpty(path))
            return;
        var client = new BlobContainerClient(_connectionString, container);
        await client.CreateIfNotExistsAsync();
        var fileName = Path.GetFileName(path);
        var blob = client.GetBlobClient(fileName);
        await blob.DeleteIfExistsAsync();

    }

    public async Task<FileStoreResult[]> Store(string container, IEnumerable<IFormFile> files)
    {
        var client = new BlobContainerClient(_connectionString, container);
        await client.CreateIfNotExistsAsync();
        client.SetAccessPolicy(PublicAccessType.Blob);

        var todos = files.Select(async file =>
        {
            var fileNameOriginal = Path.GetFileName(file.FileName);
            var extension = Path.GetExtension(file.FileName);
            var nameFile = $"{Guid.NewGuid()}{extension}";
            var blob = client.GetBlobClient(nameFile);
            var blobHttpHeadders = new BlobHttpHeaders();
            blobHttpHeadders.ContentType = file.ContentType;
            await blob.UploadAsync(file.OpenReadStream(), blobHttpHeadders);
            return new FileStoreResult
            {
                URL = blob.Uri.ToString(),
                Title = fileNameOriginal
            };
        });

        var result = await Task.WhenAll(todos);
        return result;
    }
}