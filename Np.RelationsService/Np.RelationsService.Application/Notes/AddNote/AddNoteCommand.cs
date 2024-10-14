using Np.RelationsService.Application.Abstractions.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Np.RelationsService.Application.Notes.AddNote
{
    public sealed record  AddNoteCommand(Guid NoteId) : ICommand;
}
