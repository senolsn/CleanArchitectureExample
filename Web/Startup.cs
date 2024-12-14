using System.Data;
using Application.Behaviors;
using Domain.Abstractions;
using FluentValidation;
using Infrastructure;
using Infrastructure.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

using Microsoft.OpenApi.Models;
using Web.Middleware;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Web;

public class Startup
{
    public Startup(IConfiguration configuration) => Configuration = configuration;

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        #region Presentation Katman�nda yer alan Controllerlar�, Web katman�n�n bir par�as� olarak tan�tmak.
        //Controller'lar ba�ka bir assembly'de (Presentation) tan�mland�ysa, bu metodla o assembly eklenir. 
        var presentationAssembly = typeof(Presentation.AssemblyReference).Assembly;

        services.AddControllers()
            .AddApplicationPart(presentationAssembly);
        #endregion

        #region MediatR kullan�larak CQRS Pattern'� uygulamak.
        var applicationAssembly = typeof(Application.AssemblyReference).Assembly;

        services.AddMediatR(applicationAssembly);
        #endregion

        #region MediatR pipeline'�na bir davran�� eklemek.
        /*
            IPipelineBehavior<,>: Herhangi bir Command veya Query �al��madan �nce ya da sonra ek i�lemler yapman�z� sa�lar.
            ValidationBehavior: Gelen isteklerin do�rulamas�n� yapmak i�in kullan�l�r. 
        */
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        #endregion

        #region FluentValidation k�t�phanesindeki validatorlar� otomatik olarak tespit edip. Dependency Injection Container'a ekler.
        //Application'�n Assembly (Derlenmi� Kod) i�inde validasyon s�n�flar�n� tespit ederek DI Container'a ekler.
        services.AddValidatorsFromAssembly(applicationAssembly);
        #endregion

        services.AddSwaggerGen(c =>
        {
            var presentationDocumentationFile = $"{presentationAssembly.GetName().Name}.xml";

            var presentationDocumentationFilePath =
                Path.Combine(AppContext.BaseDirectory, presentationDocumentationFile);

            c.IncludeXmlComments(presentationDocumentationFilePath);

            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Web", Version = "v1" });
        });

        //PostgreSQL :
        //services.AddDbContext<ApplicationDbContext>(builder => 
        //    builder.UseNpgsql(Configuration.GetConnectionString("Application")));
        
        services.AddDbContext<ApplicationDbContext>(builder =>
            builder.UseSqlServer(Configuration.GetConnectionString("Application")));


        services.AddScoped<IWebinarRepository, WebinarRepository>();

        services.AddScoped<IUnitOfWork>(
            factory => factory.GetRequiredService<ApplicationDbContext>());

        services.AddScoped<IDbConnection>(
            factory => factory.GetRequiredService<ApplicationDbContext>().Database.GetDbConnection());

        services.AddTransient<ExceptionHandlingMiddleware>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();

            app.UseSwagger();

            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Web v1"));
        }

        app.UseMiddleware<ExceptionHandlingMiddleware>();

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(endpoints => endpoints.MapControllers());
    }
}