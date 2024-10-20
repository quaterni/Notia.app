using Np.RelationsService.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Np.RelationsService.Domain.Notes
{
    public class NotesErrors
    {
        public static Error NotFound = new("[Notes]: Note not found.");

        public static Error AlreadyAdded = new("[Notes]: Note already added.");
    }
}
