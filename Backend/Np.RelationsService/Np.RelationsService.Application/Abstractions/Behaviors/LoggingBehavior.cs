
using MediatR;
using Microsoft.Extensions.Logging;
using Np.RelationsService.Domain.Abstractions;

namespace Np.RelationsService.Application.Abstractions.Behaviors;

internal partial class LoggingBehavior<TReqeust, TResponse> : IPipelineBehavior<TReqeust, TResponse>
    where TReqeust : IBaseRequest
    where TResponse : Result
{
    [LoggerMessage(Level=LogLevel.Information, Message= "Executing request {RequestName}")]
    private static partial void LogExecutingRequest(ILogger logger, string requestName);

    [LoggerMessage(Level = LogLevel.Information, Message = "Request {RequestName} executed with success result")]
    private static partial void LogSeccessResult(ILogger logger, string requestName);

    [LoggerMessage(Level = LogLevel.Error, Message = "Reqeust {RequestName} executed with error: {ErrorMessage}")]
    private static partial void LogErrorResult(ILogger logger, string requestName, string errorMessage);

    [LoggerMessage(Level = LogLevel.Error, Message = "Request {RequestName} failed with exception: {ExceptionMessage}")]
    private static partial void LogFailedReqeust(ILogger logger, string requestName, string exceptionMessage);

    private readonly ILogger<LoggingBehavior<TReqeust, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TReqeust, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TReqeust request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var reqeustName = request.GetType().Name;
        LogExecutingRequest(_logger, reqeustName);
        try
        {
            var response = await next();

            if (response.IsSuccess)
            {
                LogSeccessResult(_logger, reqeustName);
            }
            else
            {
                LogErrorResult(_logger, reqeustName, response.Error.Message);
            }

            return response;
        }
        catch (Exception ex) 
        {
            LogFailedReqeust(_logger, reqeustName, ex.Message);
            throw;
        }
    }
}
