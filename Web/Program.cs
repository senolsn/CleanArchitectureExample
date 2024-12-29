using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Web;

public class Program
{
    //Bu metot uygulama ba�lat�ld���nda �al��an ilk metottur.
    public static async Task Main(string[] args)
    {
        //CreateHostBuilder bir web projesini aya�a kald�r���mda o sitenin yay�nlanacak yani o siteyi bar�nd�raca��m�z olan ortam� olu�turur.
        var webHost = CreateHostBuilder(args).Build();

        //Veritaban� migrasyonlar�n� uygulamak i�in �a�r�lan asenkron bir metottur.
        await ApplyMigrations(webHost.Services);

        //Olu�turdu�umuz bar�nma ortam�n� kullanarak web uygulamas�n� ba�latmam�z� sa�lar.
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