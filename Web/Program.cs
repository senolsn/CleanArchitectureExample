using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Web;

public class Program
{
    //Bu metot uygulama ba�lat�ld���nda �al��an ilk metottur.
    public static async Task Main(string[] args)
    {
        //CreateHostBuilder bir web projesini ayaga kaldirdigimda o sitenin yay�nlanacak yani o siteyi barindiracagimiz olan ortam� olusturur.
        var webHost = CreateHostBuilder(args).Build();

        //Veritaban� migrasyonlar�n� uygulamak i�in cagirilan asenkron bir metottur.
        await ApplyMigrations(webHost.Services);

        //Olusturdugumuz barinma ortamini kullanarak web uygulamas�n� baslatmamizi sa�lar.
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