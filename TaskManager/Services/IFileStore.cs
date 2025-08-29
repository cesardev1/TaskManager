using TaskManager.Models;
namespace TaskManager.Services;

public interface IFileStore
{
    Task Delete(string path, string container);
    Task<FileStoreResult[]> Store(string container, IEnumerable<IFormFile> files);
}