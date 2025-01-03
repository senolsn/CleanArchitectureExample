using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Web;

public class Program
{
    //Bu metot uygulama başlatıldığında çalışan ilk metottur.
    public static async Task Main(string[] args)
    {
        //CreateHostBuilder bir web projesini ayağa kaldırığımda o sitenin yayınlanacak yani o siteyi barındıracağımız olan ortamı oluşturur.
        var webHost = CreateHostBuilder(args).Build();

        //Veritabanı migrasyonlarını uygulamak için çağrılan asenkron bir metottur.
        await ApplyMigrations(webHost.Services);

        //Oluşturduğumuz barınma ortamını kullanarak web uygulamasını başlatmamızı sağlar.
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