using System.Diagnostics.CodeAnalysis;

namespace Ytdlp.Web;

public sealed class SupportedDomains
{
    public readonly string Name;
    public readonly string Domain;
    public readonly bool IsAdultOnly;

    private SupportedDomains(string name, string domain, bool isAdultOnly)
    {
        Name = name;
        Domain = domain;
        IsAdultOnly = isAdultOnly;
    }

    public static class YouTube
    {
        public static readonly SupportedDomains Regular = new("YouTube", "youtube.com", false);
        public static readonly SupportedDomains Short = new("YouTube", "youtu.be", false);
        public static readonly SupportedDomains Music = new("YouTube Music", "music.youtube.com", false);
    }
    public static readonly SupportedDomains PornHub = new("PornHub", "pornhub.com", true); 

    public static readonly SupportedDomains[] All =
    {
        YouTube.Regular,
        YouTube.Short,
        YouTube.Music,
        PornHub,
    };

    public static bool TryGetDomain(string url, [MaybeNullWhen(false)] out SupportedDomains domains)
    {
        if (Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out var uri))
        {
            var host = uri.Host.ToLower();
            if (host.StartsWith("www.")) host = host.Substring(4);
            else if (host.EndsWith(".pornhub.com")) host = "pornhub.com";
            return null != (domains = All.FirstOrDefault(d => d.Domain == host));
        }
        Console.WriteLine("Couldn't parse url: " + url);
        domains = null;
        return false;
    }
}