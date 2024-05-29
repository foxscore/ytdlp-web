using Microsoft.EntityFrameworkCore;
using Ytdlp.Web;
using Ytdlp.Web.Data;
using Ytdlp.Web.Hubs;

namespace Ytdlp.Web;

class Program
{
    public static void Main(string[] args)
    {
        if (!Directory.Exists(Paths.DataPath))
            Directory.CreateDirectory(Paths.DataPath);
        if (!Directory.Exists(Paths.ContentPath))
            Directory.CreateDirectory(Paths.ContentPath);
        if (!Directory.Exists(Paths.ThumbnailsPath))
            Directory.CreateDirectory(Paths.ThumbnailsPath);
        CreateHostBuilder(args).Build().Run();
    }

    // EF Core uses this method at design time to access the DbContext
    public static IHostBuilder CreateHostBuilder(string[] args) => Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());
}