using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ytdlp.Web.Data;

public enum ContentType
{
    Video,
    Audio
}

public class Content
{
    public const int GuidLength = 32;
    
    [Key, MinLength(GuidLength), MaxLength(GuidLength)]
    public string Id { get; set; } = null!;
    public DateTime DownloadDate { get; set; }
    public string Title { get; set; } = null!;
    public float? Length { get; set; }
    public long Size { get; set; }
    public ContentType Type { get; set; }
    public DateTime? UploadDate { get; set; }
    public string ChannelName { get; set; }
    public string Source { get; set; } = null!;
    public string SourceMediaId { get; set; } = null!;
    [MaxLength(255)]
    public string RequestedUri { get; set; } = null!;
    public bool IsAdultContent { get; set; }
    
    [MinLength(GuidLength), MaxLength(GuidLength)]
    public string AssetGuid { get; set; } = null!;
    [MinLength(GuidLength), MaxLength(GuidLength)]
    public string? ThumbnailAssetGuid { get; set; }
}
