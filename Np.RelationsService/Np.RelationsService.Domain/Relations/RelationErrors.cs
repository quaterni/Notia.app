using Np.RelationsService.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Np.RelationsService.Domain.Relations
{
    public class RelationErrors
    {
        public static Error ItselfRelation => new("[Relation]: Note relates with itself"); 

        public static Error Duplication => new("[Relation]: Relation exists between notes");

    }
}
