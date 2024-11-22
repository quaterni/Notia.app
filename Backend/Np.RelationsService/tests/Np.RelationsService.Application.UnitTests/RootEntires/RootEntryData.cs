using Np.RelationsService.Application.UnitTests.Notes;
using Np.RelationsService.Domain.Relations;
using Np.RelationsService.Domain.RootEntries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Np.RelationsService.Application.UnitTests.RootEntires
{
    internal class RootEntryData
    {
        public static RootEntry DefaultRootEntry = RootEntry.Create(NoteData.Note_1).Value;
    }
}
