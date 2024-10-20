
namespace Np.RelationsService.Domain.Abstractions
{
    public class Result
    {
        public static Result Success() => new();

        public static Result<T> Success<T>(T value) => new(value);

        public static Result Failed(Error error) => new(error);

        public static Result<T> Failed<T>(Error error) => new(error);

        public static Result<T> Create<T>(T? value) => 
            value is not null? Success(value) : Failed<T>(Error.Null);

        private readonly Error? _error;

        public bool IsSuccess { get; }

        public bool IsFailed => !IsSuccess;

        public Error Error => _error is null ? 
            throw new ApplicationException("Accessing error when result success") : _error;

        protected Result()
        {
            IsSuccess = true;
        }

        protected Result(Error error)
        {
            _error = error;
            IsSuccess = false;
        }
    }

    public class Result<T> : Result
    {
        private T? _value;

        public T Value => _value is not null ? _value : 
            throw new ApplicationException("Accessing value when result failed");

        protected internal Result(T value) : base()
        {
            _value = value;
        }

        protected internal Result(Error error): base(error)
        {
            _value = default;
        }

        public static implicit operator Result<T>(T? value) => Create(value);
    }
}
