using MediatR;
using Np.UsersService.Core.Shared;

namespace Np.UsersService.Core.Business.Abstractions;

public interface IQuery<TValue> : IRequest<Result<TValue>>;
