using Microsoft.AspNetCore.SignalR;
using YoutubeDLSharp;

namespace Ytdlp.Web.Hubs;

public class DownloadProgressHub : Hub
{
    public static object[] BuildArgs(DownloadProgress progress, int progressRevision)
    {
        return
        [
            progressRevision,
            progress.State.ToString(),
            progress.Progress,
            progress.ETA ?? "",
            progress.Data ?? ""
        ];
    }
    
    public async Task SubscribeTo(string contentGuid)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, contentGuid);
        if (
            !ContentDownloader.GetForId(contentGuid, out var contentDownloader)
            || contentDownloader.Progress.State is DownloadState.Success or DownloadState.Error
        )
        {
            Console.WriteLine("Removed from SignalR group");
            await Clients.Client(Context.ConnectionId).SendCoreAsync("refreshPage", []);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, contentGuid);
        }
        else
        {
            Console.WriteLine("Sending initial update");
            await Clients.Client(Context.ConnectionId).SendCoreAsync("updateProgress", BuildArgs(contentDownloader.Progress, contentDownloader.ProgressRevision));
        }
    }
}