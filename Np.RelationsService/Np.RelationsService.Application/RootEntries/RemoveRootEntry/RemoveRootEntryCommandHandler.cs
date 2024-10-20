using Microsoft.EntityFrameworkCore;
using Np.RelationsService.Application.Abstractions.Messaging;
using Np.RelationsService.Application.Exceptions;
using Np.RelationsService.Domain.Abstractions;
using Np.RelationsService.Domain.Notes;
using Np.RelationsService.Domain.RootEntries;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Np.RelationsService.Application.UnitTests.RootEntires.RemoveRootEntry
{
    internal class RemoveRootEntryCommandHandler : ICommandHandler<RemoveRootEntryCommand>
    {
        private readonly IRootEntriesRepository _rootEntriesRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RemoveRootEntryCommandHandler(
            IRootEntriesRepository rootEntriesRepository,
            IUnitOfWork unitOfWork)
        {
            _rootEntriesRepository = rootEntriesRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(RemoveRootEntryCommand request, CancellationToken cancellationToken)
        {
            var rootEntry = await _rootEntriesRepository.GetRootEntryById(request.RootEntryId);

            if (rootEntry == null) 
            {
                return Result.Failed(RootEntryErrors.NotFound);
            }

            _rootEntriesRepository.Remove(rootEntry);

            try
            {
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new ConcurrencyException("DbContext throws concurrnecy exceptoins", ex);
            }
        }
    }
}
