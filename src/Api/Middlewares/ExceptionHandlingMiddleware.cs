using Api.Errors;
using System.Net;
using System.Text.Json;
using Serilog;

namespace Api.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IHostEnvironment _env;

    public ExceptionHandlingMiddleware(RequestDelegate next, IHostEnvironment env)
    {
        _next = next;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Proceed with the next middleware in the pipeline
            await _next(context);
        }
        catch (Exception ex)
        {
            // Log the exception details using Serilog
            Log.Error(ex, "An unexpected error occurred while processing the request.");

            // Set response details
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            // Create an appropriate response based on the environment (Development or Production)
            var response = _env.IsDevelopment()
                ? new ApiException(context.Response.StatusCode, ex.Message, ex.StackTrace)
                : new ApiResponse(context.Response.StatusCode, "An error occurred while processing your request.");

            // Serialize the response using System.Text.Json with camel case property names
            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var json = JsonSerializer.Serialize(response, options);

            // Write the response to the HTTP context
            await context.Response.WriteAsync(json);
        }
    }
}
