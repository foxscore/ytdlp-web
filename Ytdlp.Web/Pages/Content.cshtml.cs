using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ytdlp.Web.Data;

namespace Ytdlp.Web.Pages;

public class ContentModel(YtdlpContext context) : PageModel
{
    [BindProperty(SupportsGet = true)] public string ContentId { get; set; } = null!;
    public Content? YtdlpContent { get; private set; }
    public ContentDownloader? Downloader { get; private set; }

    public IActionResult OnGet()
    {
        var content = context.Content.Find(ContentId);
        if (content is not null)
        {
            YtdlpContent = content;
            return Page();
        }

        if (ContentDownloader.GetForId(ContentId, out var downloader))
        {
            Downloader = downloader;
            return Page();
        }
        
        return NotFound();
    }
}
