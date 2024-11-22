using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Np.NotesService.Application.Exceptions;

namespace Np.NotesService.Api.Middlewares;

internal partial class ExceptionHandlingMiddleware
{
    [LoggerMessage(LogLevel.Error, Message ="Exception occured: {ExceptionMessage}")]
    private static partial void LogOccuredException(ILogger logger, string exceptionMessage);

    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            LogOccuredException(_logger, ex.Message);

            var exceptionDetails = GetExceptionDetails(ex);

            ProblemDetails details = new ProblemDetails
            {
                Detail = exceptionDetails.Detail,
                Type = exceptionDetails.Type,
                Title = exceptionDetails.Title,
                Status = exceptionDetails.Status,
            };

            if(exceptionDetails.Errors is not null)
            {
                details.Extensions["errors"] = exceptionDetails.Errors;
            }

            context.Response.StatusCode = exceptionDetails.Status;

            await context.Response.WriteAsJsonAsync(details);
        }
    }

    private ExceptionDetails GetExceptionDetails(Exception exception)
    {
        return exception switch
        {
            ServiceRequestException serviceReqeustException => new ExceptionDetails(
                StatusCodes.Status500InternalServerError,
                "ServiceRequest",
                "Service request failed",
                "Failed while connection or taking request",
                null),
            _ => new ExceptionDetails(
                StatusCodes.Status500InternalServerError,
                "ServerError",
                "Server error",
                "An unexpected error has occurred",
                null)
        };
    }


    internal sealed record ExceptionDetails(
        int Status,
        string Type,
        string Title,
        string Detail,
        IEnumerable<object>? Errors);
}
