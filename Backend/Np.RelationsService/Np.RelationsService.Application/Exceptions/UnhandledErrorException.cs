
using Np.RelationsService.Domain.Abstractions;

namespace Np.RelationsService.Application.Exceptions;

internal class UnhandledErrorException : Exception
{
    public UnhandledErrorException(Error error) : base($"Error was unhandled: {error.Message}")
    {
        Error = error;
    }

    public Error Error { get; }
}
