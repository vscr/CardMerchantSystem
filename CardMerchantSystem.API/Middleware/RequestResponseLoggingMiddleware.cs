// CardMerchantSystem.API/Middleware/RequestResponseLoggingMiddleware.cs

using System.Diagnostics;
using System.Text;

namespace CardMerchantSystem.API.Middleware;

/// <summary>
/// Request ve Response'ları loglar
/// </summary>
public class RequestResponseLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

    public RequestResponseLoggingMiddleware(
        RequestDelegate next,
        ILogger<RequestResponseLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Correlation ID oluştur
        var correlationId = Guid.NewGuid().ToString();
        context.Items["CorrelationId"] = correlationId;
        context.Response.Headers.Add("X-Correlation-Id", correlationId);

        // Request body'yi oku
        var requestBody = await ReadRequestBodyAsync(context.Request);

        var stopwatch = Stopwatch.StartNew();

        // Response body'yi yakalamak için stream değiştir
        var originalBodyStream = context.Response.Body;
        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        try
        {
            // Log request
            _logger.LogInformation(
                "HTTP Request: {Method} {Path} {QueryString} - CorrelationId: {CorrelationId} - Body: {RequestBody}",
                context.Request.Method,
                context.Request.Path,
                context.Request.QueryString,
                correlationId,
                requestBody);

            await _next(context);

            stopwatch.Stop();

            // Response body'yi oku
            var responseBodyText = await ReadResponseBodyAsync(responseBody);

            // Log response
            _logger.LogInformation(
                "HTTP Response: {Method} {Path} - Status: {StatusCode} - Duration: {Duration}ms - CorrelationId: {CorrelationId} - Body: {ResponseBody}",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                stopwatch.ElapsedMilliseconds,
                correlationId,
                responseBodyText);

            // Response'u geri yaz
            await responseBody.CopyToAsync(originalBodyStream);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            _logger.LogError(ex,
                "HTTP Request Failed: {Method} {Path} - Duration: {Duration}ms - CorrelationId: {CorrelationId}",
                context.Request.Method,
                context.Request.Path,
                stopwatch.ElapsedMilliseconds,
                correlationId);

            throw;
        }
        finally
        {
            context.Response.Body = originalBodyStream;
        }
    }

    private async Task<string> ReadRequestBodyAsync(HttpRequest request)
    {
        request.EnableBuffering();

        using var reader = new StreamReader(
            request.Body,
            encoding: Encoding.UTF8,
            detectEncodingFromByteOrderMarks: false,
            bufferSize: 1024,
            leaveOpen: true);

        var body = await reader.ReadToEndAsync();
        request.Body.Position = 0;

        return body;
    }

    private async Task<string> ReadResponseBodyAsync(MemoryStream responseBody)
    {
        responseBody.Seek(0, SeekOrigin.Begin);
        var text = await new StreamReader(responseBody).ReadToEndAsync();
        responseBody.Seek(0, SeekOrigin.Begin);
        return text;
    }
}