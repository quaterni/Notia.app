using Microsoft.EntityFrameworkCore;
using Np.RelationsService.Application.Abstractions.Messaging;
using Np.RelationsService.Application.Exceptions;
using Np.RelationsService.Domain.Abstractions;
using Np.RelationsService.Domain.Notes;
using Np.RelationsService.Domain.RootEntries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Np.RelationsService.Application.RootEntries.AddRootEntry
{
    internal class AddRootEntryCommandHandler : ICommandHandler<AddRootEntryCommand>
    {
        private readonly INotesRepository _notesRepository;
        private readonly IRootEntriesRepository _rootEntriesRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AddRootEntryCommandHandler(
            INotesRepository notesRepository,
            IRootEntriesRepository rootEntriesRepository,
            IUnitOfWork unitOfWork)
        {
            _notesRepository = notesRepository;
            _rootEntriesRepository = rootEntriesRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(AddRootEntryCommand request, CancellationToken cancellationToken)
        {
            var note = await _notesRepository.GetNoteById(request.NoteId);

            if (note == null)
            {
                return Result.Failed(Error.Null);
            }

            if (await _rootEntriesRepository.IsEntryRoot(note))
            {
                return Result.Failed(RootEntryErrors.Duplication);
            }

            var rootEntry = RootEntry.Create(note).Value;

            _rootEntriesRepository.Add(rootEntry);
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
