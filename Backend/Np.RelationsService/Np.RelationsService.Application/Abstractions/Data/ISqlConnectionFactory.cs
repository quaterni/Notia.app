using System.Data;

namespace Np.RelationsService.Application.Abstractions.Data;

public interface ISqlConnectionFactory
{
    IDbConnection CreateConnection(); 
}