namespace Product.Api.Middleware;

public class LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var request = context.Request;
        logger.LogInformation("Request: {RequestMethod}: {RequestUrl}, QueryString: {QueryString}",
            request.Method, (request.PathBase + request.Path).Value, request.QueryString.Value);

        try
        {
            await next(context);
        }
        finally
        {
            logger.LogInformation("Response: {StatusCode}", context.Response.StatusCode);
        }
    }
}
