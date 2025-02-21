using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Application.Common.Behaviors;

public class LoggerBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggerBehavior<TRequest, TResponse>> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public LoggerBehavior(
        ILogger<LoggerBehavior<TRequest, TResponse>> logger,
        IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
        var requestName = typeof(TRequest).Name;
        var userEmail = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Anonymous";
        //var ipAddress = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();

        try
        {
            var response = await next();

            // Tüm işlemleri logla
            _logger.LogInformation(
                "İşlem: {Operation} - Kullanıcı: {User}",
                requestName.Replace("Command", "").Replace("Query", ""),
                userEmail);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                "HATA - İşlem: {Operation} - Kullanıcı: {User} - Hata: {Error}",
                requestName.Replace("Command", "").Replace("Query", ""),
                userEmail,
                ex.Message);
            throw;
        }
    }
} 