using MediatR;
using Np.UsersService.Core.Shared;

namespace Np.UsersService.Core.Business.Abstractions;

public interface ICommand : IRequest<Result>, ICommandBase;

public interface ICommand<TValue> : IRequest<Result<TValue>>, ICommandBase;

public interface ICommandBase : IBaseRequest;
