using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using System.Collections.ObjectModel;
using System.Data;

namespace Web;

public class Program
{
    //Bu metot uygulama başlatıldığında çalışan ilk metottur.
    public static async Task Main(string[] args)
    {
        //CreateHostBuilder bir web projesini ayaga kaldırdığımda o sitenin yayınlanacak yani o siteyi barindiracagımız olan ortamı olusturur.
        var builder = WebApplication.CreateBuilder(args);

        // SQL Server sink için kolon yapılandırması
        var columnOptions = new ColumnOptions
        {
            AdditionalColumns = new Collection<SqlColumn>
            {
                new SqlColumn
                {
                    ColumnName = "Operation",
                    DataType = SqlDbType.NVarChar,
                    DataLength = 50
                },
                new SqlColumn
                {
                    ColumnName = "User",
                    DataType = SqlDbType.NVarChar,
                    DataLength = 100
                }
            }
        };

        // Gereksiz kolonları kaldır
        columnOptions.Store.Remove(StandardColumn.Properties);
        columnOptions.Store.Remove(StandardColumn.MessageTemplate);
        columnOptions.Store.Remove(StandardColumn.Level);
        columnOptions.Store.Remove(StandardColumn.Exception);
        columnOptions.TimeStamp.NonClusteredIndex = true;

        // Serilog yapılandırması
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .WriteTo.File(
                path: "logs/app-.log",
                rollingInterval: RollingInterval.Day,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} | {Message}{NewLine}")
            .WriteTo.MSSqlServer(
                connectionString: builder.Configuration.GetConnectionString("Application"),
                sinkOptions: new MSSqlServerSinkOptions
                {
                    TableName = "Logs",
                    AutoCreateSqlTable = true
                },
                columnOptions: columnOptions)
            .CreateLogger();

        try
        {
            builder.Host.UseSerilog();

            var webHost = CreateHostBuilder(args).Build();

            await ApplyMigrations(webHost.Services);

            await webHost.RunAsync();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Uygulama başlatılırken hata oluştu");
        }
        finally
        {
            Log.CloseAndFlush();
        }
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