using Dapper;
using SalesManagement.Application.Dtos;
using SalesManagement.Application.Interfaces;
using SalesManagement.Application.DomainModels;
using SalesManagement.Data.Database;

namespace SalesManagement.Data.Repositories;

public class DistrictRepository(SqlConnectionFactory connectionFactory) : IDistrictRepository
{
    public async Task<IReadOnlyList<District>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT
                DistrictId AS Id,
                Name
            FROM dbo.District
            ORDER BY Name;
            """;

        await using var connection = connectionFactory.CreateConnection();

        await connection.OpenAsync(cancellationToken);

        var command = new CommandDefinition(
            sql,
            cancellationToken: cancellationToken);

        var districts = await connection.QueryAsync<District>(command);

        return districts.AsList();
    }

    public async Task<DistrictDetails?> GetDetailsAsync(
        int districtId,
        CancellationToken cancellationToken = default)
    {
        const string districtSql = """
            SELECT
                DistrictId AS Id,
                Name
            FROM dbo.District
            WHERE DistrictId = @DistrictId;
            """;

        const string salespersonSql = """
            SELECT
                sp.SalespersonId AS Id,
                sp.Name,
                dsp.Role
            FROM dbo.DistrictSalesperson dsp
            INNER JOIN dbo.Salesperson sp
                ON sp.SalespersonId = dsp.SalespersonId
            WHERE dsp.DistrictId = @DistrictId
            ORDER BY
                CASE dsp.Role
                    WHEN 'Primary' THEN 0
                    ELSE 1
                END,
                sp.Name;
            """;

        const string storeSql = """
            SELECT
                StoreId AS Id,
                Name
            FROM dbo.Store
            WHERE DistrictId = @DistrictId
            ORDER BY Name;
            """;

        await using var connection = connectionFactory.CreateConnection();

        await connection.OpenAsync(cancellationToken);

        var parameters = new { DistrictId = districtId };

        var district = await connection.QuerySingleOrDefaultAsync<District>(
            new CommandDefinition(
                districtSql,
                parameters,
                cancellationToken: cancellationToken));

        if (district is null)
        {
            return null;
        }

        var salespersons = await connection.QueryAsync<DistrictSalespersonDto>(
            new CommandDefinition(
                salespersonSql,
                parameters,
                cancellationToken: cancellationToken));

        var stores = await connection.QueryAsync<StoreDto>(
            new CommandDefinition(
                storeSql,
                parameters,
                cancellationToken: cancellationToken));

        return new DistrictDetails
        {
            Id = district.Id,
            Name = district.Name,
            Salespersons = salespersons.AsList(),
            Stores = stores.AsList()
        };
    }
}