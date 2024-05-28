using Microsoft.AspNetCore.Mvc;
using Ytdlp.Web.Data;
using Ytdlp.Web.Extensions;

namespace Ytdlp.Web.Controllers;

public class MediaController : Controller
{
    private readonly YtdlpContext _context;

    public MediaController(YtdlpContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Route("/{contentId}/thumbnail")]
    public async Task<IActionResult> GetThumbnail(string contentId)
    {
        var content = await _context.Content.FindAsync(contentId);
        if (content is null or { ThumbnailAssetGuid: null }) return NotFound();
        if (!System.IO.File.Exists($"{Paths.ThumbnailsPath}/{content.ThumbnailAssetGuid}")) return NotFound();
        var fs = System.IO.File.OpenRead($"{Paths.ThumbnailsPath}/{content.ThumbnailAssetGuid}");
        return File(fs, "image/png", content.Title.ToFilename("png"));
    }

    [HttpGet]
    [Route("/{contentId}/download")]
    public async Task<IActionResult> DownloadMedia(string contentId)
    {
        var content = await _context.Content.FindAsync(contentId);
        if (content is null or { AssetGuid: null }) return NotFound();
        if (!System.IO.File.Exists($"{Paths.ContentPath}/{content.AssetGuid}")) return NotFound();
        var fs = System.IO.File.OpenRead($"{Paths.ContentPath}/{content.AssetGuid}");
        return content.Type switch
        {
            ContentType.Audio => File(fs, "audio/mpeg", content.Title.ToFilename("mp3")),
            ContentType.Video => File(fs, "video/mp4", content.Title.ToFilename("mp4")),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    [HttpGet]
    [Route("/{contentId}/media")]
    public async Task<IActionResult> ShowMedia(string contentId)
    {
        var content = await _context.Content.FindAsync(contentId);
        if (content is null or { AssetGuid: null }) return NotFound();
        if (!System.IO.File.Exists($"{Paths.ContentPath}/{content.AssetGuid}")) return NotFound();
        return PhysicalFile($"{Paths.ContentPath}/{content.AssetGuid}",
            content.Type switch
            {
                ContentType.Audio => "audio/mpeg",
                ContentType.Video => "video/mp4",
                _ => throw new ArgumentOutOfRangeException()
            }, true);
    }
}