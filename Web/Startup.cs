using FluentValidation;
using Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using Web.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Application.Common.Behaviors;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Web;

public class Startup
{
    public Startup(IConfiguration configuration) => Configuration = configuration;

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        
        #region MediatR ve Behavior'ların Eklenmesi
        var applicationAssembly = typeof(Application.AssemblyReference).Assembly;
        services.AddMediatR(applicationAssembly);

        /*
            IPipelineBehavior<,>: Herhangi bir Command veya Query çalışmadan önce ya da sonra ek işlemler yapmanızı sağlar.
            ValidationBehavior: Gelen isteklerin doğrulamasını yapmak için kullanılır. 
        */

        //MediatR pipeline'ina bir davraniş eklemek.
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggerBehavior<,>));
        #endregion

        #region Identity Yapılandırması
        services.AddIdentity<IdentityUser, IdentityRole>(options =>
        {
            // Parola gereksinimleri
            options.Password.RequireDigit = true; // Parola sayısal karakter içermeli
            options.Password.RequireLowercase = true; // Parola küçük harf içermeli
            options.Password.RequireUppercase = true; // Parola büyük harf içermeli
            options.Password.RequireNonAlphanumeric = true; // Parola özel karakter içermeli
            options.Password.RequiredLength = 8; // Parola en az 8 karakter olmalı
            options.Lockout.MaxFailedAccessAttempts = 10; // Maximum başarısız giriş denemesi
            options.Lockout.AllowedForNewUsers = false; // Yeni kullanıcılar için kilitleme aktif mi?

            // Kullanıcı gereksinimleri
            options.User.RequireUniqueEmail = true; // Benzersiz e-posta gereksinimi
            options.SignIn.RequireConfirmedEmail = false; // E-posta doğrulaması isteniyorsa true yapın
            options.SignIn.RequireConfirmedPhoneNumber = false; // Telefon doğrulaması gerekli mi?
            options.SignIn.RequireConfirmedAccount = false;  // Hesap doğrulaması gerekli mi?
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();
        #endregion

        #region Presentation Katmaninda yer alan Controllerlari, Web katmaninin bir parçasi olarak tanitmak.
        //Controller'lar başka bir assembly'de (Presentation) tanimlandiysa, bu metodla o assembly eklenir. 
        var presentationAssembly = typeof(Presentation.AssemblyReference).Assembly;

        services.AddControllers()
            .AddApplicationPart(presentationAssembly);
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

        #region JWT Ayarları
        services.AddAuthentication(options => 
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.RequireHttpsMetadata = false; // Development için false, production'da true olmalı
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = Configuration["Jwt:Issuer"],
                ValidAudience = Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(Configuration["Jwt:SecretKey"])),
                ClockSkew = TimeSpan.Zero // Token süresini tam olarak kontrol etmek için
            };

            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Startup>>();
                    logger.LogError("Authentication failed: {Error}", context.Exception);
                    return Task.CompletedTask;
                },
                OnTokenValidated = context =>
                {
                    var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Startup>>();
                    logger.LogInformation("Token validated. Claims: {Claims}", 
                        string.Join(", ", context.Principal.Claims.Select(c => $"{c.Type}: {c.Value}")));
                    return Task.CompletedTask;
                },
                OnChallenge = context =>
                {
                    var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Startup>>();
                    logger.LogWarning("Authentication challenge: {Error}", context.Error);
                    return Task.CompletedTask;
                },
                OnMessageReceived = context =>
                {
                    var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Startup>>();
                    logger.LogInformation("Token received in header: {Token}", 
                        context.Request.Headers["Authorization"].ToString());
                    return Task.CompletedTask;
                }
            };
        });
        #endregion
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

        app.UseHttpsRedirection(); //Http isteklerini https'e çevirir.

        // Serilog request logging'i ekle
        app.UseSerilogRequestLogging(
        //    options =>
        //{
        //    options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
        //}
        );

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints => endpoints.MapControllers());
    }
}