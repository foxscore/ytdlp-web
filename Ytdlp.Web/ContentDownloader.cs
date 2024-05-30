using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.SignalR;
using YoutubeDLSharp;
using YoutubeDLSharp.Metadata;
using YoutubeDLSharp.Options;
using Ytdlp.Web.Data;
using Ytdlp.Web.Extensions;
using Ytdlp.Web.Hubs;

namespace Ytdlp.Web;

public static class ContentDownloaderContextHolder
{
    private static IHubContext<DownloadProgressHub> Hub { get; set; } = null!;
    internal static YtdlpContext Context { get; private set; } = null!;
    internal static readonly object ContextWriteLock = new();

    public static IApplicationBuilder InitializeContentDownloader(this IApplicationBuilder app)
    {
        var scope = app.ApplicationServices.CreateScope();
        
        Hub = scope.ServiceProvider.GetService<IHubContext<DownloadProgressHub>>()!;
        Context = scope.ServiceProvider.GetService<YtdlpContext>()!;

        return app;
    }

    public static async Task SendPageReload(string contentGuid) => await Hub.Clients.Group(contentGuid).SendCoreAsync("reloadPage", []);

    private static DownloadProgress? _previousProgress = null;
    public static async Task SendProgressUpdate(string contentGuid, DownloadProgress progress, int progressRevision)
    {
        if (_previousProgress is not null)
        {
            if (
                Math.Abs(progress.Progress - _previousProgress.Progress) < 0.0001f
                && progress.State == _previousProgress.State
                && progress.Data == _previousProgress.Data
                && progress.ETA == _previousProgress.ETA
            ) return;
        }
        _previousProgress = progress;
        
        Console.WriteLine($"Sending update: {contentGuid}, '{progress.Data}', {progress.Progress}, '{progress.ETA}', {progressRevision}, '{progress.State.ToString().SplitOnUpperCase()}'");
        await Hub.Clients.Group(contentGuid).SendCoreAsync("updateProgress", DownloadProgressHub.BuildArgs(progress, progressRevision));
    }

    public static async Task SendError(string contentGuid, string error)
    {
        await Hub.Clients.Group(contentGuid).SendCoreAsync("downloadError", [ error ]);
    }
}

public class ContentDownloader : Content
{
    private static readonly Dictionary<string, ContentDownloader> Instances = new();

    public static bool GetForId(string contentId, [MaybeNullWhen(false)] out ContentDownloader instance)
    {
        return Instances.TryGetValue(contentId, out instance);
    }

    public Task<Result<Content>> Task { get; private set; } = null!;
    public DownloadProgress Progress { get; private set; } = new(DownloadState.None, data: "Queued");
    public int ProgressRevision = 0;
    public bool DidDownloadMetadata = false;

    private static string GenerateGuid() => Guid.NewGuid().ToString().Replace("-", "");

    private ContentDownloader(string url, ContentType type)
    {
        Type = type;
        RequestedUri = url;
        DownloadDate = DateTime.Now;
        Id = GenerateGuid();
        AssetGuid = GenerateGuid();
        ThumbnailAssetGuid = GenerateGuid();
        
        Instances.Add(Id, this);
    }

    private static readonly Regex YtShortUrlRegex = new("https?://(?:www\\.)?youtu\\.be/([\\w-]+)(?:\\?.+)?");
    public static ContentDownloader BeginDownload(string url, ContentType type)
    {
        Console.WriteLine($"Requested download: " + url + " as " + type);
        var downloader = new ContentDownloader(url, type);
        downloader.Task = downloader.BeginDownload();
        return downloader;
    }

    private async Task<Result<Content>> BeginDownload()
    {
        try
        {
            Console.WriteLine($"{Id} : Begin");
            
            if (!SupportedDomains.TryGetDomain(RequestedUri, out var domain))
                throw new Exception("Unsupported or invalid url.");

            var contentUrl = RequestedUri;
            var regexMatch = YtShortUrlRegex.Match(contentUrl);
            if (regexMatch.Success)
            {
                contentUrl = $"https://youtube.com/watch?v={regexMatch.Groups[1].Value}";
                Console.WriteLine("Converted short YouTube link to: " + contentUrl);
            }

            var ytdl = new YoutubeDL
            {
                YoutubeDLPath = Paths.YtdlpPath,
                FFmpegPath = Paths.FfmpegPath,
                OutputFolder = $"/tmp/{AssetGuid}/",
                OutputFileTemplate = "output.%(ext)s",
                OverwriteFiles = true,
            };
            Directory.CreateDirectory(ytdl.OutputFolder);

            #region Fetch metadata

            Console.WriteLine($"{Id} : Fetch metadata");
            Progress = new DownloadProgress(DownloadState.PreProcessing, data: "Downloading metadata");
            await ContentDownloaderContextHolder.SendProgressUpdate(
                Id,
                Progress = new DownloadProgress(DownloadState.PreProcessing, data: "Downloading metadata"),
                ++ProgressRevision
            );
            var metadataResult = await ytdl.RunVideoDataFetch(contentUrl);
            if (!metadataResult.Success)
                throw new Exception("Failed to fetch metadata.");
            Console.WriteLine($"{Id} : Got it, parsing");
            var meta = metadataResult.Data!;
            if (meta.Availability != null && meta.Availability is not Availability.Public &&
                meta.Availability is not Availability.Unlisted)
                throw new Exception($"Cannot access {meta.Availability.ToString()} content.");
            if (meta.ResultType is not MetadataType.Video)
                throw new Exception("Provided link is not a video");

            Title = meta.Title;
            Length = meta.Duration;
            UploadDate = meta.UploadDate;
            ChannelName = meta.Uploader;
            Source = domain.Name;
            SourceMediaId = meta.ID;
            IsAdultContent = domain.IsAdultOnly || meta.AgeLimit is >= 18;

            if (string.IsNullOrEmpty(meta.Thumbnail))
                ThumbnailAssetGuid = null;
            else
            {
                Console.WriteLine($"{Id} : Downloading thumbnail");
                try
                {
                    using var client = new HttpClient();
                    await using var stream = await client.GetStreamAsync(meta.Thumbnail);
                    await using var file = new FileStream("/tmp/" + ThumbnailAssetGuid, FileMode.CreateNew);
                    await stream.CopyToAsync(file);
                    Console.WriteLine($"{Id} : Done");
                }
                catch
                {
                    // ToDo: Log
                    Console.WriteLine($"{Id} : Failed");
                    ThumbnailAssetGuid = null;
                }
            }
            
            DidDownloadMetadata = true;
            _ = ContentDownloaderContextHolder.SendPageReload(Id);

            #endregion

            #region Download content
            var progress = new Progress<DownloadProgress>(p =>
            {
                if (p.State is DownloadState.Success) return;
                _ = ContentDownloaderContextHolder.SendProgressUpdate(Id, Progress = p, ++ProgressRevision);
            });

            Console.WriteLine($"{Id} : Starting download");
            var result = Type switch
            {
                ContentType.Video => await ytdl.RunVideoDownload(
                    contentUrl,
                    recodeFormat: VideoRecodeFormat.Mp4,
                    progress: progress,
                    overrideOptions: new()
                    {
                        NoSponsorblock = true,
                    }
                ),
                ContentType.Audio => await ytdl.RunAudioDownload(
                    contentUrl,
                    AudioConversionFormat.Mp3,
                    progress: progress
                ),
                _ => throw new ArgumentOutOfRangeException(),
            };
            if (!result.Success)
                throw new Exception("Failed to download content:\n\t" + result.ErrorOutput.Aggregate((left, right) => $"{left}\n\t {right}"));
            Console.WriteLine($"{Id} : Got it, parsing");
            var filePath = result.Data!;
            Size = new FileInfo(filePath).Length;

            #endregion

            #region Cleanup

            await ContentDownloaderContextHolder.SendProgressUpdate(
                Id,
                Progress = new DownloadProgress(DownloadState.PostProcessing, data: "Cleaning up", progress: Progress.Progress),
                ++ProgressRevision
            );
            Console.WriteLine($"{Id} : Saving");
            File.Move($"/tmp/{ThumbnailAssetGuid}", $"{Paths.ThumbnailsPath}/{ThumbnailAssetGuid}");
            File.Move(filePath, $"{Paths.ContentPath}/{AssetGuid}");

            lock (ContentDownloaderContextHolder.ContextWriteLock)
            {
                ContentDownloaderContextHolder.Context.Content.Add(this);
                ContentDownloaderContextHolder.Context.SaveChanges();
            }

            #endregion

            await ContentDownloaderContextHolder.SendProgressUpdate(
                Id,
                Progress = new DownloadProgress(DownloadState.Success, data: "Done!"),
                ++ProgressRevision
            );
            Console.WriteLine($"{Id} : All done");
            await ContentDownloaderContextHolder.SendPageReload(Id);
            return Result<Content>.Success(this);
        }
        catch (Exception e)
        {
            Console.WriteLine($"{Id} : FAILED");
            Console.WriteLine(e.ToString());
            Instances.Remove(Id);
            new Thread(() =>
            {
                if (Directory.Exists("/tmp/" + AssetGuid))
                    Directory.Delete("/tmp/" + AssetGuid, true);
                if (File.Exists("/tmp/" + ThumbnailAssetGuid))
                    File.Delete("/tmp/" + ThumbnailAssetGuid);
            }).Start();
            await ContentDownloaderContextHolder.SendError(Id, e.Message);
            return Result<Content>.Failure(e);
        }
    }
}