namespace backend.Common.Storage;

public class ImageStorageOptions
{
    public const string SectionName = "ImageStorage";

    /// <summary>
    /// Headroom added on top of the file size for multipart boundaries, headers
    /// and the other form fields that travel with an upload. Shared by the
    /// Kestrel/form limits in Program.cs and by ImageUploadSizeLimitAttribute so
    /// the two cannot drift.
    /// </summary>
    public const long MultipartOverheadBytes = 65_536;

    public long MaxFileSizeBytes { get; set; } = 5_242_880;

    public string[] AllowedExtensions { get; set; } =
        [".jpg", ".jpeg", ".png", ".webp", ".gif"];
}
