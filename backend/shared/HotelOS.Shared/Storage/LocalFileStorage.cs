namespace HotelOS.Shared.Storage;

public sealed class LocalFileStorage(string rootPath) : IFileStorage
{
    public async Task<string> SaveAsync(string folder, string fileName, Stream content, CancellationToken cancellationToken = default)
    {
        var folderPath = Path.Combine(rootPath, folder);
        Directory.CreateDirectory(folderPath);

        var filePath = Path.Combine(folderPath, fileName);
        await using var fileStream = File.Create(filePath);
        await content.CopyToAsync(fileStream, cancellationToken);
        return filePath;
    }

    public Task<byte[]> ReadAsync(string path, CancellationToken cancellationToken = default)
        => File.ReadAllBytesAsync(path, cancellationToken);
}