using System.Globalization;
using System.Net;
using System.Text.Json;
using Northwind.Services.Repositories;

namespace Northwind.Orders.WebApi.Middlewares;
public class ResponseError
{
    private readonly JsonSerializerOptions options;
    private string[] messages = default!;

    public ResponseError()
    {
        this.options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
        };
    }

    public string? StatusCode { get; set; }

    public IReadOnlyList<string>? Messages
    {
        get => this.messages != null ? Array.AsReadOnly(this.messages) : null;
        set => this.messages =
            value?.ToArray() ??
            [];
    }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this, this.options);
    }
}

public class ExceptionMiddleware
{
    private readonly RequestDelegate next;
    private readonly ILogger<ExceptionMiddleware> logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        this.next = next;
        this.logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await this.next(httpContext);
        }
        catch (OrderNotFoundException ex)
        {
            await this.SetError(
                                httpContext,
                                HttpStatusCode.NotFound,
                                ex,
                                ["Order is not found!"]);
        }
        catch (Exception ex)
        {
            await this.SetError(
                httpContext,
                HttpStatusCode.InternalServerError,
                ex,
                ["Internal server error occured."]);
        }
    }

    private async Task SetError(HttpContext httpContext, HttpStatusCode httpStatusCode, Exception ex, string[] messages)
    {
        if (httpContext != null)
        {
            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = (int)httpStatusCode;
            var responseError = new ResponseError
            {
                StatusCode = httpContext.Response.StatusCode.ToString(CultureInfo.InvariantCulture),
                Messages = messages,
            };
            this.logger.LogError(ex, "An error occurred: {ResponseError}", responseError.ToString());
            await httpContext.Response.WriteAsync(responseError.ToString());
        }
    }
}

public static class ExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExceptionMiddleware>();
    }
}
