
namespace Np.UsersService.Core.Shared;

public class Result
{
    private readonly Error? _error;

    internal protected Result()
    {
        IsSuccess = true; 
    }

    internal protected Result(Error error)
    {
        _error = error;
    }

    public bool IsSuccess { get; init; }

    public bool IsFailed => !IsSuccess;

    public Error Error => IsSuccess ? 
        throw new ApplicationException("Trying access error when result is success") : 
        _error!;

    public static Result Success() => new Result();

    public static Result<TValue> Success<TValue>(TValue value) => new Result<TValue>(value);

    public static Result Failure(Error error) => new Result(error);

    public static Result<TValue> Failure<TValue>(Error error) => new Result<TValue>(error);
}

public class Result<TValue> : Result
{
    private readonly TValue? _value;

    internal protected Result(TValue value) : base()
    {
        _value = value;
    }

    internal protected Result(Error error) : base(error)
    {
    }

    public TValue Value => IsSuccess ?
        _value! :
        throw new ApplicationException("Trying access value when result is failed");

    public static implicit operator Result<TValue>(TValue result) => Success<TValue>(result);
}
