using TaskManager.Models;

namespace TaskManager.Services;

public class LocalFilesRepository : IFileStore
{
    private readonly IWebHostEnvironment _env;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public LocalFilesRepository(IWebHostEnvironment env,IHttpContextAccessor httpContextAccessor)
    {
        _env = env;
        _httpContextAccessor = httpContextAccessor;
    }
    public Task Delete(string path, string container)
    {
        if(string.IsNullOrEmpty(path))
            return Task.CompletedTask;
        var nameFile = Path.GetFileName(path);
        var pathFile = Path.Combine(_env.WebRootPath, container, nameFile);
        if(File.Exists(pathFile))
            File.Delete(pathFile);
        
        return Task.CompletedTask;
    }

    public async Task<FileStoreResult[]> Store(string container, IEnumerable<IFormFile> files)
    {
        var todos = files.Select(async file =>
        {
            var nameFileOriginal = Path.GetFileName(file.FileName);
            var extension = Path.GetExtension(file.FileName);
            var nameFile = $"{Guid.NewGuid()}{extension}";
            string folder = Path.Combine(_env.WebRootPath, container);
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            string path = Path.Combine(folder, nameFile);
            using (var ms = new MemoryStream())
            {
                await file.CopyToAsync(ms);
                var content = ms.ToArray();
                await File.WriteAllBytesAsync(path, content);
            }

            var url = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}";
            var urlFile  = Path.Combine(url,container,nameFile).Replace("\\","/");
            
            return new FileStoreResult
            {
                URL = urlFile,
                Title = nameFileOriginal
            };
        });
        
        var result = await Task.WhenAll(todos);
        return result;
    }
}