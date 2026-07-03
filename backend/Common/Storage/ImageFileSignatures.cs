namespace backend.Common.Storage;

public static class ImageFileSignatures
{
    public static bool MatchesExtension(ReadOnlySpan<byte> header, int bytesRead, string extension)
    {
        if (bytesRead < 3)
            return false;

        return extension.ToLowerInvariant() switch
        {
            ".png" => bytesRead >= 8 &&
                      header[0] == 0x89 &&
                      header[1] == 0x50 &&
                      header[2] == 0x4E &&
                      header[3] == 0x47 &&
                      header[4] == 0x0D &&
                      header[5] == 0x0A &&
                      header[6] == 0x1A &&
                      header[7] == 0x0A,
            ".jpg" or ".jpeg" => header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF,
            ".gif" => bytesRead >= 6 &&
                      header[0] == (byte)'G' &&
                      header[1] == (byte)'I' &&
                      header[2] == (byte)'F' &&
                      header[3] == (byte)'8' &&
                      (header[4] == (byte)'7' || header[4] == (byte)'9') &&
                      header[5] == (byte)'a',
            ".webp" => bytesRead >= 12 &&
                       header[0] == (byte)'R' &&
                       header[1] == (byte)'I' &&
                       header[2] == (byte)'F' &&
                       header[3] == (byte)'F' &&
                       header[8] == (byte)'W' &&
                       header[9] == (byte)'E' &&
                       header[10] == (byte)'B' &&
                       header[11] == (byte)'P',
            _ => false
        };
    }
}
