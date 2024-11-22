using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Np.NotesService.Application.Abstractions.Data
{
    public interface ISqlConnectionFactory
    {
        public IDbConnection CreateConnection();
    }
}
