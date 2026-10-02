using Microsoft.Data.SqlClient;

namespace SalesManagement.Data.Database;

public sealed class SqlConnectionFactory(string connectionString)
{
    public SqlConnection CreateConnection()
    {
        return new SqlConnection(connectionString);
    }
}