using MediatR;
using Np.UsersService.Core.Shared;

namespace Np.UsersService.Core.Business.Abstractions;

public interface IQueryHandler<TQuery, TValue> : IRequestHandler<TQuery, Result<TValue>>
    where TQuery : IQuery<TValue>;
