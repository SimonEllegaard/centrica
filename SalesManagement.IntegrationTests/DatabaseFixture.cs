using Microsoft.Data.SqlClient;
using SalesManagement.Data.Database;
using SalesManagement.Data.Repositories;

namespace SalesManagement.IntegrationTests;

public sealed class DatabaseFixture
{
    private const string ConnectionString =
        "Server=DESKTOP-OL6L16D;" +
        "Database=SalesManagement;" +
        "Trusted_Connection=True;" +
        "TrustServerCertificate=True;";

    private SqlConnectionFactory ConnectionFactory { get; }

    public DistrictRepository DistrictRepository { get; }

    public DatabaseFixture()
    {
        ConnectionFactory = new SqlConnectionFactory(ConnectionString);
        DistrictRepository = new DistrictRepository(ConnectionFactory);
    }

    public async Task VerifyDatabaseIsAvailableAsync()
    {
        await using var connection =
            new SqlConnection(ConnectionString);

        await connection.OpenAsync();
    }
}