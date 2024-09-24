using MediatR;
using Np.NotesService.Domain.Abstractions;

namespace Np.NotesService.Application.Abstractions.Mediator
{
    public interface IQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
        where TQuery : IQuery<TResponse>;
}
