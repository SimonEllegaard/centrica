using Dapper;
using Microsoft.Data.SqlClient;
using SalesManagement.Application.Dtos;
using SalesManagement.Application.Interfaces;
using SalesManagement.Data.Database;

namespace SalesManagement.Data.Repositories;

public class SalespersonRepository(SqlConnectionFactory connectionFactory)
    : ISalespersonRepository
{
    public async Task<IReadOnlyList<Salesperson>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        const string sql = """
                           SELECT
                               SalespersonId AS Id,
                               Name
                           FROM dbo.Salesperson
                           ORDER BY Name;
                           """;

        await using var connection = await OpenConnectionAsync(cancellationToken);

        var command = new CommandDefinition(
            sql,
            cancellationToken: cancellationToken);

        var salespeople = await connection.QueryAsync<Salesperson>(command);

        return salespeople.AsList();
    }

    private async Task<SqlConnection> OpenConnectionAsync(
        CancellationToken cancellationToken)
    {
        var connection = connectionFactory.CreateConnection();

        await connection.OpenAsync(cancellationToken);

        return connection;
    }
}