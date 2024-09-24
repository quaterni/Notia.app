using Np.NotesService.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Np.NotesService.Domain.Notes.Events
{
    public sealed record NoteCreatedDomainEvent(Guid NoteId) : IDomainEvent;
}
