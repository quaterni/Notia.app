using MediatR;
using Np.NotesService.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Np.NotesService.Application.Abstractions.Mediator
{
    public interface IQuery<TResponse> : IRequest<Result<TResponse>>;
}
