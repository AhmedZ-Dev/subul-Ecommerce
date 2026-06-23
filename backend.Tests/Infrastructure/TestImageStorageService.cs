using backend.Common.Results;
using backend.Common.Storage;
using Microsoft.AspNetCore.Http;

namespace backend.Tests.Infrastructure;

public class TestImageStorageService(string rootPath) : IImageStorageService
{
    private static readonly string[] AllowedExtensions =
        [".jpg", ".jpeg", ".png", ".webp", ".gif"];

    private const long MaxFileSizeBytes = 5_242_880;

    public async Task<Result<string>> SaveProductImageAsync(
        long productId,
        IFormFile file,
        CancellationToken cancellationToken)
    {
        if (file.Length == 0)
            return Result<string>.Failure("Image file is required");

        if (file.Length > MaxFileSizeBytes)
            return Result<string>.Failure("Image file exceeds maximum allowed size");

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (string.IsNullOrEmpty(extension) ||
            !AllowedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
            return Result<string>.Failure("Invalid image file");

        var fileName = $"{Guid.NewGuid():N}{extension}";
        var relativePath = $"/img/products/{productId}/{fileName}";
        var physicalPath = GetPhysicalPath(relativePath);

        Directory.CreateDirectory(Path.GetDirectoryName(physicalPath)!);

        await using var stream = new FileStream(
            physicalPath,
            FileMode.CreateNew,
            FileAccess.Write,
            FileShare.None);

        await file.CopyToAsync(stream, cancellationToken);

        return Result<string>.Success(relativePath);
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

        return Path.GetFullPath(Path.Combine(rootPath, relativeFromImg));
    }
}
