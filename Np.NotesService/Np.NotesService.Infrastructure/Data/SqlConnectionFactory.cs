using Np.NotesService.Application.Abstractions.Data;
using Npgsql;
using System.Data;


namespace Np.NotesService.Infrastructure.Data
{
    internal class SqlConnectionFactory : ISqlConnectionFactory
    {
        private readonly string _connectionString;

        public SqlConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IDbConnection CreateConnection()
        {
            var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            return connection;
        }
    }
}
