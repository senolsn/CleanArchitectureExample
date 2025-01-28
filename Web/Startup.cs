using System.Data;
using Application.Behaviors;
using Domain.Abstractions;
using FluentValidation;
using Infrastructure;
using Infrastructure.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

using Microsoft.OpenApi.Models;
using Web.Middleware;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Infrastructure.Authentication;
using Application.Abstractions.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Web;

public class Startup
{
    public Startup(IConfiguration configuration) => Configuration = configuration;

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        #region Presentation Katmaninda yer alan Controllerlari, Web katmaninin bir parçasi olarak tanitmak.
        //Controller'lar başka bir assembly'de (Presentation) tanimlandiysa, bu metodla o assembly eklenir. 
        var presentationAssembly = typeof(Presentation.AssemblyReference).Assembly;

        services.AddControllers()
            .AddApplicationPart(presentationAssembly);
        #endregion

        #region MediatR kullanilarak CQRS Pattern'i uygulamak.
        var applicationAssembly = typeof(Application.AssemblyReference).Assembly;

        services.AddMediatR(applicationAssembly);
        #endregion

        #region MediatR pipeline'ina bir davraniş eklemek.
        /*
            IPipelineBehavior<,>: Herhangi bir Command veya Query çalışmadan önce ya da sonra ek işlemler yapmanızı sağlar.
            ValidationBehavior: Gelen isteklerin doğrulamasını yapmak için kullanılır. 
        */
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        #endregion

        #region FluentValidation kutuphanesindeki validatorlari otomatik olarak tespit edip. Dependency Injection Container'a ekler.
        //Application'ın Assembly (Derlenmiş Kod) içinde validasyon sınıflarını tespit ederek DI Container'a ekler.
        services.AddValidatorsFromAssembly(applicationAssembly);
        #endregion

        #region Swagger'ı yapılandırmak için kullanılan kodlar.
        services.AddSwaggerGen(c =>
        {
            var presentationDocumentationFile = $"{presentationAssembly.GetName().Name}.xml";

            var presentationDocumentationFilePath =
                Path.Combine(AppContext.BaseDirectory, presentationDocumentationFile);

            c.IncludeXmlComments(presentationDocumentationFilePath);

            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Web", Version = "v1" });
        });
        #endregion

        #region Veritabanı ayarları
        //PostgreSQL :
        //services.AddDbContext<ApplicationDbContext>(builder => 
        //    builder.UseNpgsql(Configuration.GetConnectionString("Application")));

        services.AddDbContext<ApplicationDbContext>(builder =>
            builder.UseSqlServer(Configuration.GetConnectionString("Application")));
        #endregion

        #region Invertion of Control Container yapılandırılması.
        services.AddInfrastructureServices(Configuration);
        services.AddWebServices();
        #endregion

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration["Jwt:Issuer"],
                    ValidAudience = Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(Configuration["Jwt:SecretKey"]))
                };
            });
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

        app.UseAuthentication();

        app.UseAuthorization();

        app.UseEndpoints(endpoints => endpoints.MapControllers());
    }
}