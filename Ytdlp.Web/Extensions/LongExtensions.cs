namespace Ytdlp.Web.Extensions;

public static class LongExtensions
{
    public static string ToByteSize(this long bytes)
    {
        string[] suffixes = { "B", "KB", "MB", "GB", "TB", "PB", "EB" };
        if (bytes == 0)
            return $"0 {suffixes[0]}"; // Handle 0 bytes

        int magnitude = (int)Math.Log(bytes, 1024); // Calculate magnitude (power of 1024)
        double adjustedSize = (double)bytes / (1L << (magnitude * 10));  // Adjust size

        // Format with two decimal places and appropriate suffix
        return $"{adjustedSize:N2} {suffixes[magnitude]}";
    }
}
