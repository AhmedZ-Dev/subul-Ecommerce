namespace backend.Common.Storage;

public class ImageStorageOptions
{
    public const string SectionName = "ImageStorage";

    public long MaxFileSizeBytes { get; set; } = 5_242_880;

    public string[] AllowedExtensions { get; set; } =
        [".jpg", ".jpeg", ".png", ".webp", ".gif"];
}
