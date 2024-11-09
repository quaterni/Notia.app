namespace Np.UsersService.Core.Exceptions;

public class ConcurrentException : Exception
{
    public ConcurrentException() : base("Concurrent exception was thrown")
    {
    }

    public ConcurrentException(Exception innerException) : base("Concurrent exception was thrown", innerException)
    {
    }
}
