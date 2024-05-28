using Microsoft.AspNetCore.Mvc;
using Ytdlp.Web.Data;
using Ytdlp.Web.Models;

namespace Ytdlp.Web.Controllers;

public class DownloadController : Controller
{
    [HttpPost]
    [Route("/api/initiateDownload")]
    public IActionResult InitiateDownload([FromBody] InitiateDownloadModel model)
    {
        Console.WriteLine("Url: " + model.url);
        Console.WriteLine("Type: " + model.type);

        if (model.url.StartsWith("http://")) model.url = "https" + model.url.Substring(4);
        else if (!model.url.StartsWith("https://")) model.url = "https://" + model.url;

        if (model.url.Length > 255)
            return UnprocessableEntity("Links may not be longer than 255 characters");
        
        if (!SupportedDomains.TryGetDomain(model.url, out _))
            return UnprocessableEntity("Unsupported URL");

        ContentType parsedType;
        switch (model.type)
        {
            case "audio":
                parsedType = ContentType.Audio;
                break;
            case "video":
                parsedType = ContentType.Video;
                break;
            default:
                return UnprocessableEntity($"Invalid type '{model.type}', expected 'audio' or 'video'!");
        }
        
        var downloader = ContentDownloader.BeginDownload(model.url, parsedType);
        return Ok(downloader.Id);
    }
}
