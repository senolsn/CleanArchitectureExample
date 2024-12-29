using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Web;

public class Program
{
    //Bu metot uygulama baþlatýldýðýnda çalýþan ilk metottur.
    public static async Task Main(string[] args)
    {
        //CreateHostBuilder bir web projesini ayaða kaldýrýðýmda o sitenin yayýnlanacak yani o siteyi barýndýracaðýmýz olan ortamý oluþturur.
        var webHost = CreateHostBuilder(args).Build();

        //Veritabaný migrasyonlarýný uygulamak için çaðrýlan asenkron bir metottur.
        await ApplyMigrations(webHost.Services);

        //Oluþturduðumuz barýnma ortamýný kullanarak web uygulamasýný baþlatmamýzý saðlar.
        await webHost.RunAsync();
    }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());

    private static async Task ApplyMigrations(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();

        await using var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await dbContext.Database.MigrateAsync();
    }
}