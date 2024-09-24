using Np.NotesService.Application.Abstractions.Mediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Np.NotesService.Application.Notes.GetNote
{
    public sealed record GetNoteQuery(Guid Id) : IQuery<GetNoteResponse>;
}
