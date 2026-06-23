using backend.Common.Storage;
using Microsoft.AspNetCore.Http;

namespace backend.Tests.Infrastructure;

public static class ProductImageTestHelpers
{
    private static readonly byte[] MinimalPngBytes =
    [
        0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A,
        0x00, 0x00, 0x00, 0x0D, 0x49, 0x48, 0x44, 0x52,
        0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x01,
        0x08, 0x06, 0x00, 0x00, 0x00, 0x1F, 0x15, 0xC4,
        0x89, 0x00, 0x00, 0x00, 0x0A, 0x49, 0x44, 0x41,
        0x54, 0x78, 0x9C, 0x63, 0x00, 0x01, 0x00, 0x00,
        0x05, 0x00, 0x01, 0x0D, 0x0A, 0x2D, 0xB4, 0x00,
        0x00, 0x00, 0x00, 0x49, 0x45, 0x4E, 0x44, 0xAE,
        0x42, 0x60, 0x82
    ];

    public static TestImageStorageService CreateStorageService()
    {
        var root = Path.Combine(Path.GetTempPath(), "subul-test-img", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        return new TestImageStorageService(root);
    }

    public static IFormFile CreatePngFormFile(string fileName = "test.png")
    {
        var stream = new MemoryStream(MinimalPngBytes);
        return new FormFile(stream, 0, stream.Length, "Image", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = "image/png"
        };
    }

    public static IFormFile CreateInvalidFormFile(string fileName = "test.txt")
    {
        var bytes = "not an image"u8.ToArray();
        var stream = new MemoryStream(bytes);
        return new FormFile(stream, 0, stream.Length, "Image", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = "text/plain"
        };
    }
}
