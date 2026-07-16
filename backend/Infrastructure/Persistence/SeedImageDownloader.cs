using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using backend.Common.Storage;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace backend.Infrastructure.Persistence;

public static partial class SeedImageDownloader
{
    private static readonly HttpClient Http = new() { Timeout = TimeSpan.FromSeconds(30) };
    private static readonly Dictionary<string, string> Cache = new(StringComparer.OrdinalIgnoreCase);

    private const string FallbackPhotoId = "1597872200969-2b65d56bd16b";

    private static readonly Dictionary<string, string> PhotoIdAliases = new(StringComparer.OrdinalIgnoreCase)
    {
        ["1587825140708-287a6e0e0e0e"] = "1541140532154-b024d705b90a",
        ["1612815154628-70f0a0b0b0b0"] = "1541140532154-b024d705b90a",
        ["1615663676942-6f2b8b8b8b8b"] = "1527814050087-3793815479db",
        ["1531492040000-000000000000"] = "1597872200969-2b65d56bd16b",
        ["1625948538000-000000000000"] = "1597872200969-2b65d56bd16b",
        ["1562976548100-000000000000"] = "1593640408182-31c70c8268f5",
        ["1591798932910-000000000000"] = "1593640408182-31c70c8268f5",
    };

    public static string GetImgRoot(IWebHostEnvironment env) =>
        Path.GetFullPath(Path.Combine(env.ContentRootPath, "..", "img"));

    public static string BuildUnsplashUrl(string photoId) =>
        $"https://images.unsplash.com/photo-{NormalizePhotoId(photoId)}?w=800&q=80";

    public static string NormalizePhotoId(string photoId) =>
        PhotoIdAliases.TryGetValue(photoId, out var alias) ? alias : photoId;

    public static async Task<string> ResolveSeedPhotoAsync(
        IWebHostEnvironment env,
        string photoId,
        CancellationToken cancellationToken = default)
    {
        var resolved = await TryResolveSeedPhotoAsync(env, photoId, cancellationToken);
        return resolved ?? await TryResolveSeedPhotoAsync(env, FallbackPhotoId, cancellationToken)
            ?? $"/img/seed/{NormalizePhotoId(FallbackPhotoId)}.jpg";
    }

    public static async Task<string?> TryMigrateExternalUrlAsync(
        IWebHostEnvironment env,
        string externalUrl,
        ILogger? logger = null,
        CancellationToken cancellationToken = default)
    {
        if (!externalUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            return externalUrl;

        if (Cache.TryGetValue(externalUrl, out var cached))
            return cached;

        try
        {
            var photoId = TryExtractUnsplashPhotoId(externalUrl);
            if (photoId is not null)
            {
                var path = await TryResolveSeedPhotoAsync(env, photoId, cancellationToken);
                if (path is not null)
                {
                    Cache[externalUrl] = path;
                    return path;
                }

                logger?.LogWarning("Could not migrate Unsplash image {Url}, using fallback", externalUrl);
                var fallback = await TryResolveSeedPhotoAsync(env, FallbackPhotoId, cancellationToken);
                if (fallback is not null)
                {
                    Cache[externalUrl] = fallback;
                    return fallback;
                }

                return null;
            }

            var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(externalUrl)))[..16]
                .ToLowerInvariant();
            var relativePath = $"/img/migrated/{hash}.jpg";
            var imgRoot = GetImgRoot(env);
            var physicalPath = Path.Combine(imgRoot, "migrated", $"{hash}.jpg");

            if (!File.Exists(physicalPath))
            {
                Directory.CreateDirectory(Path.Combine(imgRoot, "migrated"));
                if (!await TryDownloadAndSaveAsync(externalUrl, physicalPath, cancellationToken))
                {
                    logger?.LogWarning("Could not download external image {Url}", externalUrl);
                    return null;
                }
            }

            Cache[externalUrl] = relativePath;
            return relativePath;
        }
        catch (Exception ex)
        {
            logger?.LogWarning(ex, "Failed to migrate external image {Url}", externalUrl);
            return null;
        }
    }

    private static async Task<string?> TryResolveSeedPhotoAsync(
        IWebHostEnvironment env,
        string photoId,
        CancellationToken cancellationToken)
    {
        var normalizedId = NormalizePhotoId(photoId);

        if (Cache.TryGetValue(normalizedId, out var cached))
            return cached;

        var relativePath = $"/img/seed/{normalizedId}.jpg";
        var imgRoot = GetImgRoot(env);
        var physicalPath = Path.Combine(imgRoot, "seed", $"{normalizedId}.jpg");

        if (!File.Exists(physicalPath))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(physicalPath)!);
            if (!await TryDownloadAndSaveAsync(BuildUnsplashUrl(normalizedId), physicalPath, cancellationToken))
                return null;
        }

        Cache[normalizedId] = relativePath;
        Cache[photoId] = relativePath;
        return relativePath;
    }

    private static string? TryExtractUnsplashPhotoId(string url)
    {
        var match = UnsplashPhotoIdRegex().Match(url);
        return match.Success ? NormalizePhotoId(match.Groups[1].Value) : null;
    }

    private static async Task<bool> TryDownloadAndSaveAsync(
        string url,
        string physicalPath,
        CancellationToken cancellationToken)
    {
        try
        {
            using var response = await Http.GetAsync(url, cancellationToken);
            if (!response.IsSuccessStatusCode)
                return false;

            var bytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);
            if (!ImageFileSignatures.MatchesExtension(bytes, bytes.Length, ".jpg"))
                return false;

            await File.WriteAllBytesAsync(physicalPath, bytes, cancellationToken);
            return true;
        }
        catch
        {
            return false;
        }
    }

    [GeneratedRegex(@"photo-(\d+-[a-f0-9]+)", RegexOptions.IgnoreCase)]
    private static partial Regex UnsplashPhotoIdRegex();
}
