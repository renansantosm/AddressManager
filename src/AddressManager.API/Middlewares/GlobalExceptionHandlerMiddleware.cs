using AddressManager.Domain.Exceptions;

namespace AddressManager.API.Middlewares;

public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

    public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var requestPath = context.Request.Path;
        var requestMethod = context.Request.Method;

        try
        {
            await _next(context);
        }
        catch (FluentValidation.ValidationException ex)
        {
            _logger.LogWarning("Validation failed - {RequestMethod} {RequestPath}", requestMethod, requestPath);

            var errors = ex.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray());

            context.Response.StatusCode = StatusCodes.Status400BadRequest;

            await context.Response.WriteAsJsonAsync(new { Errors = errors });
        }
        catch (AddressNotFoundException ex)
        {
            _logger.LogInformation("Address not found on {RequestMethod} {RequestPath}", requestMethod, requestPath);

            context.Response.StatusCode = StatusCodes.Status404NotFound;
            await context.Response.WriteAsJsonAsync(new { Error = ex.Message });
        }
        catch (DomainValidationException ex)
        {
            _logger.LogInformation("Domain validation failed - {RequestMethod} {RequestPath}", requestMethod, requestPath);

            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new { Error = ex.Message });
        }
        catch (ExternalServiceException ex)
        {
            _logger.LogError("External service error - {RequestMethod} {RequestPath}", requestMethod, requestPath);

            context.Response.StatusCode = StatusCodes.Status502BadGateway;
            await context.Response.WriteAsJsonAsync(new { Error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred - {RequestMethod} {RequestPath}", requestMethod, requestPath);

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(new { Error = "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde." });
        }
    }
}
