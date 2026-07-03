using backend.Common.Results;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace backend.Common.Storage;

public class LocalImageStorageService(
    IWebHostEnvironment environment,
    IOptions<ImageStorageOptions> options) : IImageStorageService
{
    private readonly ImageStorageOptions _options = options.Value;

    public Task<Result<string>> SaveProductImageAsync(
        long productId,
        IFormFile file,
        CancellationToken cancellationToken) =>
        SaveImageAsync($"products/{productId}", $"{Guid.NewGuid():N}", file, cancellationToken);

    public Task<Result<string>> SaveBrandImageAsync(
        long brandId,
        string slot,
        IFormFile file,
        CancellationToken cancellationToken)
    {
        if (slot is not ("logo" or "banner"))
            return Task.FromResult(Result<string>.Failure("Invalid brand image slot"));

        return SaveImageAsync($"brands/{brandId}", $"{slot}-{Guid.NewGuid():N}", file, cancellationToken);
    }

    private async Task<Result<string>> SaveImageAsync(
        string folder,
        string fileBaseName,
        IFormFile file,
        CancellationToken cancellationToken)
    {
        if (file.Length == 0)
            return Result<string>.Failure("Image file is required");

        if (file.Length > _options.MaxFileSizeBytes)
            return Result<string>.Failure("Image file exceeds maximum allowed size");

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (string.IsNullOrEmpty(extension) ||
            !_options.AllowedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
            return Result<string>.Failure("Invalid image file");

        await using var inputStream = file.OpenReadStream();
        using var buffer = new MemoryStream();
        await inputStream.CopyToAsync(buffer, cancellationToken);

        if (!ValidateImageContent(buffer, extension))
            return Result<string>.Failure("Invalid image file");

        var fileName = $"{fileBaseName}{extension}";
        var relativePath = $"/img/{folder}/{fileName}";
        var physicalPath = GetPhysicalPath(relativePath);

        Directory.CreateDirectory(Path.GetDirectoryName(physicalPath)!);

        await using var stream = new FileStream(
            physicalPath,
            FileMode.CreateNew,
            FileAccess.Write,
            FileShare.None);

        buffer.Position = 0;
        await buffer.CopyToAsync(stream, cancellationToken);

        return Result<string>.Success(relativePath);
    }

    private static bool ValidateImageContent(MemoryStream buffer, string extension)
    {
        buffer.Position = 0;
        var header = new byte[12];
        var bytesRead = buffer.Read(header, 0, header.Length);
        buffer.Position = 0;
        return ImageFileSignatures.MatchesExtension(header, bytesRead, extension);
    }

    public Task DeleteByRelativePathAsync(
        string relativePath,
        CancellationToken cancellationToken)
    {
        var physicalPath = GetPhysicalPath(relativePath);

        if (File.Exists(physicalPath))
            File.Delete(physicalPath);

        return Task.CompletedTask;
    }

    private string GetPhysicalPath(string relativePath)
    {
        var normalized = relativePath
            .Replace('\\', '/')
            .TrimStart('/');

        if (!normalized.StartsWith("img/", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Invalid image path");

        var relativeFromImg = normalized["img/".Length..];
        if (relativeFromImg.Contains("..", StringComparison.Ordinal))
            throw new InvalidOperationException("Invalid image path");

        var imgRoot = Path.GetFullPath(Path.Combine(environment.ContentRootPath, "..", "img"));
        var fullPath = Path.GetFullPath(Path.Combine(imgRoot, relativeFromImg));

        if (!fullPath.StartsWith(imgRoot + Path.DirectorySeparatorChar, StringComparison.Ordinal) &&
            !fullPath.Equals(imgRoot, StringComparison.Ordinal))
            throw new InvalidOperationException("Invalid image path");

        return fullPath;
    }
}
