using MediatR;
using Np.NotesService.Domain.Abstractions;


namespace Np.NotesService.Application.Abstractions.Mediator
{
    public interface ICommandHandler<TCommand> : IRequestHandler<TCommand, Result>
        where TCommand : ICommand;

    public interface ICommandHandler<TCommand, TResponse> : IRequestHandler<TCommand, Result<TResponse>>
        where TCommand : ICommand<TResponse>;
}
