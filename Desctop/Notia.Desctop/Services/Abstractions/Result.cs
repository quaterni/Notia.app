
using System;
using System.Diagnostics.CodeAnalysis;

namespace Notia.Desctop.Services.Abstractions;

internal class Result
{
    private readonly Error? _error = null;
    private readonly bool _isSuccess;

    protected internal Result()
    {        
    }

    protected internal Result(Error error)
    {
        _error = error;
        _isSuccess = false;
    }

    public bool IsSuccess { get; set; }

    public bool IsFailed => !IsSuccess;

    [NotNull]
    public Error Error => IsFailed? 
        _error! : 
        throw new InvalidOperationException("Trying get error when result success");

    public static Result Success() => new Result();

    public static Result Failure(Error error) => new Result(error);

    public static Result<TValue> Success<TValue>(TValue value) => new Result<TValue>(value);

    public static Result<TValue> Failure<TValue>(Error error) => new Result<TValue>(error);

    public static Result<TValue> Create<TValue>(TValue? value) =>
        value is not null ? Success(value) : Failure<TValue>(Error.NullValue);
}

internal class Result<TValue> : Result
{
    private readonly TValue? _value = default;

    protected internal Result(TValue value): base()
    {
        _value = value;
    }

    protected internal Result(Error error) :base(error)
    {
    }

    [NotNull]
    public TValue Value => IsSuccess ?
        _value! :
        throw new InvalidOperationException("Trying get value when result success");

    public static implicit operator Result<TValue>(TValue? result)=> Create(result);
}