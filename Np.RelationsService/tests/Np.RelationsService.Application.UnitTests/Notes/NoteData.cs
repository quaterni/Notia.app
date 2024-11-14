using Np.RelationsService.Domain.Notes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Np.RelationsService.Application.UnitTests.Notes
{
    public class NoteData
    {
        public static Note Note_1 = Note.Create(new Guid("6bcf248d-312b-4a75-b54a-0a477885b76d"), Guid.Empty).Value;
        public static Note Note_1_1 = Note.Create(new Guid("4bcf568d-312b-4a75-b54a-0a477885b76d"), Guid.Empty).Value;
        public static Note Note_1_2 = Note.Create(new Guid("6bcf708d-312b-4a75-b54a-0a477885b76d"), Guid.Empty).Value;
        public static Note Note_0 = Note.Create(new Guid("0bcf708d-312b-4a75-b54a-0a477885b76d"), Guid.Empty).Value;
    }
}
