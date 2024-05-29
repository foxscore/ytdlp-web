using Microsoft.EntityFrameworkCore;
using Ytdlp.Web.Data;
using Ytdlp.Web.Hubs;

namespace Ytdlp.Web;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContext<YtdlpContext>(options =>
        {
            options.UseSqlite(
                $"Filename={Paths.DataPath}/ytdlp.db",
                b => b.MigrationsAssembly(typeof(Program).Assembly.FullName)
            );
        });

        // Add services to the container.
        services.AddControllersWithViews();
        services.AddRazorPages();
        services.AddSignalR();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // Configure the HTTP request pipeline.
        if (!env.IsDevelopment())
        {
            // app.UseExceptionHandler("/error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }
        else app.UseDeveloperExceptionPage();

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseEndpoints(e =>
        {
            e.MapControllers();
            e.MapRazorPages();
            e.MapHub<DownloadProgressHub>("/hubs/downloadProgress");
        });

        app.InitializeContentDownloader();
    }
}