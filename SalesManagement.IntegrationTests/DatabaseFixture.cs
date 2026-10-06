using Microsoft.Extensions.Configuration;
using SalesManagement.Data.Database;
using SalesManagement.Data.Repositories;

namespace SalesManagement.IntegrationTests;

public class DatabaseFixture
{
    public SqlConnectionFactory ConnectionFactory { get; }

    public DistrictRepository DistrictRepository { get; }

    public DatabaseFixture()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.Testing.json", optional: false)
            .Build();

        var connectionString =
            configuration.GetConnectionString("SalesManagement")
            ?? throw new InvalidOperationException(
                "Connection string 'SalesManagement' was not found.");

        ConnectionFactory = new SqlConnectionFactory(connectionString);
        DistrictRepository = new DistrictRepository(ConnectionFactory);
    }

    public async Task VerifyDatabaseIsAvailableAsync()
    {
        await using var connection = ConnectionFactory.CreateConnection();

        await connection.OpenAsync();
    }
}