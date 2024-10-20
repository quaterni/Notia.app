using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Np.NotesService.Domain.Abstractions
{
    public record Result
    {
        public static Result Success() => new();

        public static Result Failure(string message) => new(message);

        public static Result<TValue> Success<TValue>(TValue value) => new(value);

        public static Result<TValue> Failure<TValue>(string message) => new(message);

        public static Result<TValue> Create<TValue>(TValue? value) =>
            value is not null ? Success(value) : Failure<TValue>("Null value error");

        private string _message = string.Empty;

        protected internal Result(string message)
        {
            IsSuccess = false;
            _message = message;
        }

        public Result()
        {
            IsSuccess = true;
        }

        public bool IsSuccess { get; }

        public bool IsFailed => !IsSuccess;

        public string Message => _message;
    }

    public record Result<TValue> : Result
    {
        private readonly TValue? _value;

        protected internal Result(string message) : base(message)
        {
            _value = default;
        }

        protected internal Result(TValue? value) : base()
        {
            _value = value;
        }

        [NotNull]
        public TValue Value => IsSuccess ? _value! : throw new InvalidOperationException("Accessing value from failed result.");

        public static implicit operator Result<TValue>(TValue? value) => Create(value);

    }
}
