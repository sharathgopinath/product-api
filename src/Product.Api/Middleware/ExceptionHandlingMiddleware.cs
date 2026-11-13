using FluentValidation;
using Product.Api.Exceptions;

namespace Product.Api.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            if (context.Response.HasStarted)
            {
                logger.LogError(ex, "Unhandled exception after the response started; cannot write ProblemDetails.");
                throw;
            }

            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var extensions = new Dictionary<string, object?> { ["traceId"] = context.TraceIdentifier };

        IResult result;
        switch (ex)
        {
            case ValidationException validationException:
                var errors = validationException.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
                logger.LogWarning("Validation failed for {Path}: {Errors}", context.Request.Path,
                    string.Join("; ", validationException.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}")));
                result = Results.ValidationProblem(errors, extensions: extensions);
                break;

            case BadHttpRequestException badRequestException:
                logger.LogWarning("Bad request: {Message}", badRequestException.Message);
                result = Results.Problem(
                    detail: badRequestException.Message,
                    statusCode: badRequestException.StatusCode,
                    extensions: extensions);
                break;

            case NotFoundException notFoundException:
                logger.LogWarning("Not found: {Message}", notFoundException.Message);
                result = Results.Problem(
                    detail: notFoundException.Message,
                    statusCode: StatusCodes.Status404NotFound,
                    extensions: extensions);
                break;

            case ConflictException conflictException:
                logger.LogWarning("Conflict: {Message}", conflictException.Message);
                result = Results.Problem(
                    detail: conflictException.Message,
                    statusCode: StatusCodes.Status409Conflict,
                    extensions: extensions);
                break;

            default:
                logger.LogError(ex, "Unhandled exception");
                result = Results.Problem(
                    title: "An unexpected error occurred.",
                    statusCode: StatusCodes.Status500InternalServerError,
                    extensions: extensions);
                break;
        }

        await result.ExecuteAsync(context);
    }
}
