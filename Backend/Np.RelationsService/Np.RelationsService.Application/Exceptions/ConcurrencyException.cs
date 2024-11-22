using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Np.RelationsService.Application.Exceptions
{
    public class ConcurrencyException : Exception
    {
        public ConcurrencyException()
        {
        }

        public ConcurrencyException(string? message) : base(message)
        {
        }

        public ConcurrencyException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
