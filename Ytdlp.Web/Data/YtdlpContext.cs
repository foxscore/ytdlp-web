using Microsoft.EntityFrameworkCore;

namespace Ytdlp.Web.Data;

public class YtdlpContext(DbContextOptions<YtdlpContext> options) : DbContext(options)
{
    public DbSet<Content> Content { get; set; }
}