using Microsoft.Data.SqlClient;

namespace SalesManagement.Data.Database;

public class SqlConnectionFactory(string connectionString)
{
    public SqlConnection CreateConnection()
    {
        return new SqlConnection(connectionString);
    }
}