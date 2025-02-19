using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Web;

public class Program
{
    //Bu metot uygulama başlatıldığında çalışan ilk metottur.
    public static async Task Main(string[] args)
    {
        //CreateHostBuilder bir web projesini ayaga kaldırdığımda o sitenin yayınlanacak yani o siteyi barindiracagımız olan ortamı olusturur.
        var builder = WebApplication.CreateBuilder(args);

        // Serilog yapılandırması
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)  // appsettings.json'dan oku
            .CreateLogger();

        builder.Host.UseSerilog();  // Host'a Serilog'u ekle

        var webHost = CreateHostBuilder(args).Build();

        await ApplyMigrations(webHost.Services);

        await webHost.RunAsync();
    }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseSerilog()  // Burada da ekleyelim
            .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());

    private static async Task ApplyMigrations(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();

        await using var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await dbContext.Database.MigrateAsync();
    }
}