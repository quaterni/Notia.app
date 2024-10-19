using Microsoft.AspNetCore.Mvc;

namespace Np.RelationsService.Api.Middlewares;

public partial class CustomExceptionHandlingMiddleware
{
    [LoggerMessage(Level = LogLevel.Error, Message ="Request executed with exception: {ExceptionMessage}")]
    private static partial void LogErrorReqeust(ILogger logger, string exceptionMessage);

    private readonly RequestDelegate _requestDelegate;
    private readonly ILogger<CustomExceptionHandlingMiddleware> _logger;

    public CustomExceptionHandlingMiddleware(RequestDelegate requestDelegate, ILogger<CustomExceptionHandlingMiddleware> logger)
    {
        _requestDelegate = requestDelegate;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _requestDelegate(context);
        }
        catch (Exception ex) 
        {
            var problemDetails = GetProblemDetails(ex);

            context.Response.StatusCode = problemDetails.Status;

            await context.Response.WriteAsJsonAsync(
                new ProblemDetails
                {
                    Title = problemDetails.Title,
                    Status = problemDetails.Status,
                    Detail = problemDetails.Details,
                });
        }
    }

    private ExceptionDetails GetProblemDetails(Exception exception)
    {
        return exception switch
        {
            _ => new ExceptionDetails(
                500,
                "Server error",
                "Internal server error")
        };
    }

    internal sealed record ExceptionDetails(
        int Status, 
        string Title, 
        string Details);
}
