using System.Text.Json;
using Application.Common.Exceptions;
using Domain.Exceptions.Base;

namespace Web.Middleware;

internal sealed class ExceptionHandlingMiddleware : IMiddleware
{
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger) => _logger = logger;

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception e)
        {
            _logger.LogError("Hata: {ErrorType} - {ErrorMessage}", e.GetType().Name, e.Message);
            await HandleExceptionAsync(context, e);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
    {
        var response = new ErrorResponse
        {
            Status = GetStatusCode(exception),
            Message = exception.Message,
            Errors = GetErrors(exception)
        };

        httpContext.Response.ContentType = "application/json";
        httpContext.Response.StatusCode = response.Status;

        await httpContext.Response.WriteAsync(JsonSerializer.Serialize(response));
    }

    private static int GetStatusCode(Exception exception) => exception switch
    {
        BadRequestException or ValidationException => StatusCodes.Status400BadRequest,
        NotFoundException => StatusCodes.Status404NotFound,
        _ => StatusCodes.Status500InternalServerError
    };

    private static IEnumerable<ApiError> GetErrors(Exception exception)
    {
        if (exception is ValidationException validationException)
        {
            return validationException.Errors
                .SelectMany(kvp => kvp.Value, (kvp, value) => new ApiError(kvp.Key, value));
        }
        return Array.Empty<ApiError>();
    }

    private record ApiError(string PropertyName, string ErrorMessage);
    private class ErrorResponse
    {
        public int Status { get; set; }
        public string Message { get; set; }
        public IEnumerable<ApiError> Errors { get; set; }
    }
}