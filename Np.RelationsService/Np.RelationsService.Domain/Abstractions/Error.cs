using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Np.RelationsService.Domain.Abstractions
{
    public record Error(string Message)
    {
        public static Error Null => new("Null value error");

        public static Error Undefined => new("Undefined error");
    }
}
