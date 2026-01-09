using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using gymBackendC__.WebApi.Models.DTO;
using Microsoft.IdentityModel.Tokens;

namespace gymBackendC__.WebApi.Middleware;

public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, title) = MapExceptionToStatusCode(exception);
        context.Response.StatusCode = (int)statusCode;
        
        var response = new ExceptionModel()
        {
            Error = title,
            Message = GetExceptionDetail(exception)
        };
        logger.LogInformation("{code} error: {title}, more: {message}", statusCode, title, exception.Message);
        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
    
    private static (int StatusCode, string Title) MapExceptionToStatusCode(Exception exception)
    {
        return exception switch
        {
            ValidationException => (StatusCodes.Status400BadRequest, "Validation Error"),
            ArgumentException or ArgumentNullException => (StatusCodes.Status400BadRequest, "Bad Request"),
            InvalidOperationException => (StatusCodes.Status400BadRequest, "Invalid Operation"),
            UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Not logged into account"),
            SecurityTokenException => (StatusCodes.Status403Forbidden, "Not had enough permissions"),
            KeyNotFoundException => (StatusCodes.Status404NotFound, "Not found"),
            OperationCanceledException => (StatusCodes.Status408RequestTimeout, "Request Timeout"),
            DbUpdateConcurrencyException => (StatusCodes.Status409Conflict, "Concurrency Conflict"),
            _ => (StatusCodes.Status500InternalServerError, "Internal Server Error")
        };
    }
    
    private static string GetExceptionDetail(Exception exception)
    {
        return exception switch
        {
            ValidationException => exception.Message,
            ArgumentException => exception.Message,
            KeyNotFoundException => exception.Message,
            DbUpdateConcurrencyException => exception.Message,
            _ => "An error occurred while processing your request"
        };
    }
}