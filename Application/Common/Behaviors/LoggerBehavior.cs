using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Text.Json;

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
        var uniqueId = Guid.NewGuid().ToString();
        var user = _httpContextAccessor.HttpContext?.User;
        
        // Debug için tüm claim'leri logla
        _logger.LogDebug("Mevcut Claims: {@Claims}", 
            user?.Claims.Select(c => new { c.Type, c.Value }).ToList());
            
        // Claim'leri doğru şekilde oku
        var userEmail = user?.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value ?? "Anonymous";
        var userId = user?.FindFirst("jti")?.Value ?? "Anonymous";


        var operationType = DetermineOperationType(requestName);
        var timestamp = DateTime.UtcNow;
        var ipAddress = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();
        var userAgent = _httpContextAccessor.HttpContext?.Request?.Headers["User-Agent"].ToString();
        
        try
        {
            // Request başlangıç logu
            _logger.LogInformation(
                "İşlem Başladı: {@LogDetail}",
                new
                {
                    OperationId = uniqueId,
                    Timestamp = timestamp,
                    OperationType = operationType,
                    RequestName = requestName,
                    UserId = userId,
                    UserEmail = userEmail,
                    IpAddress = ipAddress,
                    UserAgent = userAgent,
                    RequestDetails = SerializeRequest(request)
                });

            var timer = Stopwatch.StartNew();
            var response = await next();
            timer.Stop();

            // İşlem başarılı logu
            _logger.LogInformation(
                "İşlem Tamamlandı: {@LogDetail}",
                new
                {
                    OperationId = uniqueId,
                    Timestamp = timestamp,
                    CompletedAt = DateTime.UtcNow,
                    Duration = $"{timer.ElapsedMilliseconds:N0}ms",
                    OperationType = operationType,
                    RequestName = requestName,
                    UserId = userId,
                    UserEmail = userEmail,
                    IpAddress = ipAddress,
                    UserAgent = userAgent,
                    RequestDetails = SerializeRequest(request),
                    ResponseDetails = SerializeResponse(response),
                    Status = "Success"
                });

            return response;
        }
        catch (Exception ex)
        {
            // Hata logu
            _logger.LogError(
                ex,
                "İşlem Başarısız: {@LogDetail}",
                new
                {
                    OperationId = uniqueId,
                    Timestamp = timestamp,
                    FailedAt = DateTime.UtcNow,
                    OperationType = operationType,
                    RequestName = requestName,
                    UserId = userId,
                    UserEmail = userEmail,
                    IpAddress = ipAddress,
                    UserAgent = userAgent,
                    RequestDetails = SerializeRequest(request),
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace,
                    Status = "Failed",
                    ErrorType = ex.GetType().Name
                });
            throw;
        }
    }

    private string DetermineOperationType(string requestName)
    {
        if (requestName.EndsWith("Command"))
        {
            if (requestName.StartsWith("Create")) return "Create";
            if (requestName.StartsWith("Update")) return "Update";
            if (requestName.StartsWith("Delete")) return "Delete";
            return "Command";
        }
        
        if (requestName.EndsWith("Query"))
        {
            if (requestName.StartsWith("Get")) return "Get";
            if (requestName.StartsWith("List")) return "List";
            if (requestName.StartsWith("Search")) return "Search";
            return "Query";
        }

        return "Other";
    }

    private string SerializeRequest(TRequest request)
    {
        try
        {
            // Hassas verileri maskeleme
            var requestJson = JsonSerializer.Serialize(request, new JsonSerializerOptions
            {
                WriteIndented = true,
                MaxDepth = 3
            });

            // Hassas verileri maskele
            requestJson = MaskSensitiveData(requestJson);

            return requestJson;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Request serialization failed");
            return "Request serialization failed";
        }
    }

    private string SerializeResponse(TResponse response)
    {
        try
        {
            if (response == null) return "No response data";

            var responseJson = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                WriteIndented = true,
                MaxDepth = 3
            });

            // Hassas verileri maskele
            responseJson = MaskSensitiveData(responseJson);

            return responseJson;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Response serialization failed");
            return "Response serialization failed";
        }
    }

    private string MaskSensitiveData(string json)
    {
        // Hassas veri içeren alanları maskele
        var sensitiveFields = new[] { "password", "token", "secret", "creditCard", "ssn" };
        
        foreach (var field in sensitiveFields)
        {
            var pattern = $@"""{field}"":\s*""[^""]+""";
            json = System.Text.RegularExpressions.Regex.Replace(
                json,
                pattern,
                $@"""{field}"": ""***MASKED***""",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        }

        return json;
    }
} 