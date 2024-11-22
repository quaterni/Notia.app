using MediatR;
using Np.UsersService.Core.Shared;

namespace Np.UsersService.Core.Business.Behaviors;

internal partial class LoggingBehavoir<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IBaseRequest
    where TResponse : Result
{
    [LoggerMessage(Level = LogLevel.Information, Message = "Executing request {RequestName}")]
    static partial void LogExecutingMessage(ILogger logger, string requestName);

    [LoggerMessage(Level = LogLevel.Information, Message = "Request processed successfully {RequestName}")]
    static partial void LogSuccessMessage(ILogger logger, string requestName);

    [LoggerMessage(Level = LogLevel.Error, Message = "Request processed with error {RequestName}\n Error message: {Message}")]
    static partial void LogErrorMessage(ILogger logger, string requestName, string message);

    [LoggerMessage(Level = LogLevel.Error, Message = "Request processed failed {RequestName}\n Exception message: {Message}")]
    static partial void LogFailMessage(ILogger logger, string requestName, string message);

    private readonly ILogger<LoggingBehavoir<TRequest, TResponse>> _logger;

    public LoggingBehavoir(ILogger<LoggingBehavoir<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = request.GetType().Name;
        try
        {
            LogExecutingMessage(_logger, requestName);

            TResponse result = await next();

            if (result.IsSuccess)
            {
                LogSuccessMessage(_logger, requestName);
            }
            else
            {
                LogErrorMessage(_logger, requestName, result.Error.Name);
            }

            return result;
        }
        catch (Exception ex)
        {
            LogFailMessage(_logger, requestName, ex.Message);
            throw;
        }
    }
}