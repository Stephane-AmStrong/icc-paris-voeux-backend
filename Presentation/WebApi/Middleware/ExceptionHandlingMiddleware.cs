using System.Text.Json;
using Application.Common;
using Domain.Errors;

namespace WebApi.Middleware;

internal sealed class ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception e)
        {
            LogException(e);
            await HandleExceptionAsync(context, e);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
    {
        // Avoid overwriting an already-started answer
        if (httpContext.Response.HasStarted) return;

        httpContext.Response.Clear();
        httpContext.Response.ContentType = "application/json";

        var (statusCode, message) = GetErrorDetails(exception);
        httpContext.Response.StatusCode = statusCode;

        object response;
        if (exception is ValidationError validationEx)
        {
            response = new
            {
                Title = message,
                Status = statusCode,
                validationEx.Errors,
                TraceId = httpContext.TraceIdentifier
            };
        }
        else
        {
            response = new
            {
                Error = new
                {
                    Message = message,
                    Type = exception.GetType().Name,
                    TraceId = httpContext.TraceIdentifier
                }
            };
        }

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        await httpContext.Response.WriteAsync(JsonSerializer.Serialize(response, options));
    }

    private static (int StatusCode, string Message) GetErrorDetails(Exception exception)
    {
        return exception switch
        {
            ValidationError => (StatusCodes.Status400BadRequest, exception.Message),
            BadRequestException => (StatusCodes.Status400BadRequest, exception.Message),
            BadHttpRequestException => (StatusCodes.Status400BadRequest, exception.Message),
            NotFoundException => (StatusCodes.Status404NotFound, exception.Message),
            JsonException => (StatusCodes.Status400BadRequest, "Invalid JSON format"),
            _ => (StatusCodes.Status500InternalServerError, "An internal server error occurred")
        };
    }

    private void LogException(Exception e)
    {
        if (e is ValidationError validationEx)
        {
            logger.LogWarning("Validation error: {Errors}",
                string.Join(", ", validationEx.Errors.SelectMany(err => err.Value.Select(msg => $"[{err.Key}] {msg}")))
            );
        }
        else
        {
            logger.LogError(e, "An unhandled exception occurred: {Message}", e.Message);
        }
    }
}
