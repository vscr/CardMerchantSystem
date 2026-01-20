using System.Net;
using System.Text.Json;
using CardMerchantSystem.API.Models;
using CardMerchantSystem.Shared.Kernel.Exceptions;

namespace CardMerchantSystem.API.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public GlobalExceptionMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionMiddleware> logger,
        IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var traceId = context.TraceIdentifier;
        var path = context.Request.Path;

        var (statusCode, errorResponse) = exception switch
        {
            NotFoundException ex => (
                HttpStatusCode.NotFound,
                ApiErrorResponse.Create(ex.Code, ex.Message, (int)HttpStatusCode.NotFound, traceId, path)),

            ValidationException ex => (
                HttpStatusCode.BadRequest,
                new ApiErrorResponse
                {
                    Code = ex.Code,
                    Message = ex.Message,
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Errors = ex.Errors,
                    TraceId = traceId,
                    Path = path
                }),

            BusinessRuleException ex => (
                HttpStatusCode.UnprocessableEntity,
                ApiErrorResponse.Create(ex.Code, ex.Message, (int)HttpStatusCode.UnprocessableEntity, traceId, path)),

            ConflictException ex => (
                HttpStatusCode.Conflict,
                ApiErrorResponse.Create(ex.Code, ex.Message, (int)HttpStatusCode.Conflict, traceId, path)),

            UnauthorizedException ex => (
                HttpStatusCode.Unauthorized,
                ApiErrorResponse.Create(ex.Code, ex.Message, (int)HttpStatusCode.Unauthorized, traceId, path)),

            ForbiddenException ex => (
                HttpStatusCode.Forbidden,
                ApiErrorResponse.Create(ex.Code, ex.Message, (int)HttpStatusCode.Forbidden, traceId, path)),

            DomainException ex => (
                HttpStatusCode.BadRequest,
                ApiErrorResponse.Create(ex.Code, ex.Message, (int)HttpStatusCode.BadRequest, traceId, path)),

            _ => (
                HttpStatusCode.InternalServerError,
                ApiErrorResponse.Create(
                    "INTERNAL_ERROR",
                    _environment.IsDevelopment() ? exception.Message : "Beklenmeyen bir hata oluştu",
                    (int)HttpStatusCode.InternalServerError,
                    traceId,
                    path))
        };

        // Loglama
        LogException(exception, statusCode, traceId, path);

        // Response yazma
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = _environment.IsDevelopment()
        };

        var json = JsonSerializer.Serialize(errorResponse, jsonOptions);
        await context.Response.WriteAsync(json);
    }

    private void LogException(Exception exception, HttpStatusCode statusCode, string traceId, string path)
    {
        var logLevel = statusCode switch
        {
            HttpStatusCode.InternalServerError => LogLevel.Error,
            HttpStatusCode.BadRequest => LogLevel.Warning,
            HttpStatusCode.NotFound => LogLevel.Information,
            _ => LogLevel.Warning
        };

        _logger.Log(
            logLevel,
            exception,
            "HTTP {StatusCode} - {ExceptionType} - TraceId: {TraceId} - Path: {Path} - Message: {Message}",
            (int)statusCode,
            exception.GetType().Name,
            traceId,
            path,
            exception.Message);
    }
}