using Np.UsersService.Core.Shared;

namespace Np.UsersService.Core.Exceptions;

public class UnhandledErrorException : Exception
{
    private const string DefaultMessage = "Error was unhandled {0}";

    public UnhandledErrorException(Error error) : base(string.Format(DefaultMessage, error.Name))
    {
        Error = error;
    }

    public UnhandledErrorException(Error error, string? message) : base(message)
    {
        Error = error;
    }

    public UnhandledErrorException(Error error, string? message, Exception? innerException) : base(message, innerException)
    {
        Error = error;
    }

    public Error Error { get; }
}
