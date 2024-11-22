using MediatR;
using Np.RelationsService.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Np.RelationsService.Application.Abstractions.Messaging
{
    public interface ICommand : IBaseCommand, IRequest<Result>;

    public interface ICommand<T> : IBaseCommand, IRequest<Result<T>>;

    public interface IBaseCommand;
}
