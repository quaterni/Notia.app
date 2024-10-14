using MediatR;
using Np.RelationsService.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Np.RelationsService.Application.Abstractions.Messaging
{
    internal interface ICommandHandler<TCommand> : IRequestHandler<TCommand, Result> 
        where TCommand: ICommand;

    internal interface ICommandHandler<TCommand, TResponse> : IRequestHandler<TCommand, Result<TResponse>> 
        where TCommand : ICommand<TResponse>;
}
