namespace HotelOS.Shared.Storage;

public interface IFileStorage
{
    Task<string> SaveAsync(string folder, string fileName, Stream content, CancellationToken cancellationToken = default);
    Task<byte[]> ReadAsync(string path, CancellationToken cancellationToken = default);
}