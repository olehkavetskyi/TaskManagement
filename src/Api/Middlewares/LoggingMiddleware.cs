using System.Diagnostics;
using System.Text;

namespace Api.Middlewares;

public class LoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LoggingMiddleware> _logger;

    public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        // Log request details
        var requestBody = await ReadRequestBodyAsync(context.Request);
        _logger.LogInformation("Request received: {Method} {Path} {Body} at {Time}",
            context.Request.Method, context.Request.Path, requestBody, DateTime.UtcNow);

        // Intercept and capture the response body
        var originalResponseBodyStream = context.Response.Body;

        using var newResponseBodyStream = new MemoryStream();
        context.Response.Body = newResponseBodyStream;

        await _next(context); // Call the next middleware

        // Log response details
        var responseBody = await ReadResponseBodyAsync(newResponseBodyStream);
        _logger.LogInformation("Response sent: {StatusCode} {Body} in {Duration}ms",
            context.Response.StatusCode, responseBody, stopwatch.ElapsedMilliseconds);

        // Reset the response body stream
        await newResponseBodyStream.CopyToAsync(originalResponseBodyStream);
    }

    private async Task<string> ReadRequestBodyAsync(HttpRequest request)
    {
        request.EnableBuffering();
        using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
        var body = await reader.ReadToEndAsync();
        request.Body.Position = 0; // Reset the request body stream for other middlewares
        return body;
    }

    private async Task<string> ReadResponseBodyAsync(Stream responseBodyStream)
    {
        responseBodyStream.Seek(0, SeekOrigin.Begin);
        var body = await new StreamReader(responseBodyStream).ReadToEndAsync();
        responseBodyStream.Seek(0, SeekOrigin.Begin); // Reset the stream for writing back to the original response
        return body;
    }
}
