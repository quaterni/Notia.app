using Np.UsersService.Core.Shared;

namespace Np.UsersService.Core.Exceptions;

public class ValidationException : Exception
{
    public ValidationException(IEnumerable<Error> errors)
    {
        Errors = errors;
    }

    public IEnumerable<Error> Errors { get; set; }
}
