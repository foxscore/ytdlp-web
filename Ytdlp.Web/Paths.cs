using System.Diagnostics;

namespace Ytdlp.Web;

public static class Paths
{
#if DEBUG
    public static readonly string DataPath = Path.GetFullPath("./bin_data");
#else
    public static readonly string DataPath = "/data";
#endif
    public static readonly string ContentPath = $"{DataPath}/content";
    public static readonly string ThumbnailsPath = $"{DataPath}/thumbnails";
    
    private static string GetPathFor(string cmd)
    {
        if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            throw new NotImplementedException();
        
        var processInfo = new ProcessStartInfo
        {
            FileName = "/bin/bash",
            Arguments = $"-c \"which {cmd}\"",
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(processInfo);

        process?.WaitForExit();
        var output = process?.StandardOutput.ReadToEnd().Trim();
        if (output is null) throw new NullReferenceException("Path not found for " + cmd);
        return output;
    }
    
    public static readonly string YtdlpPath = GetPathFor("yt-dlp");
    public static readonly string FfmpegPath = GetPathFor("ffmpeg");
    
}
