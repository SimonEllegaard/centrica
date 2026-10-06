using Dapper;
using Microsoft.Data.SqlClient;
using SalesManagement.Application.Dtos;
using SalesManagement.Application.Interfaces;
using SalesManagement.Application.DomainModels;
using SalesManagement.Application.DomainModels.Enums;
using SalesManagement.Data.Database;
using DistrictSalesperson = SalesManagement.Application.Dtos.DistrictSalesperson;
using Store = SalesManagement.Application.Dtos.Store;

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

        await using var connection = await OpenConnectionAsync(cancellationToken);

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

        await using var connection = await OpenConnectionAsync(cancellationToken);

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

        var salespersons = await connection.QueryAsync<DistrictSalesperson>(
            new CommandDefinition(
                salespersonSql,
                parameters,
                cancellationToken: cancellationToken));

        var stores = await connection.QueryAsync<Store>(
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

    public async Task<bool> DistrictExistsAsync(
        int districtId,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
                           SELECT 1
                           FROM dbo.District
                           WHERE DistrictId = @DistrictId;
                           """;

        await using var connection = await OpenConnectionAsync(cancellationToken);

        var command = new CommandDefinition(
            sql,
            new { DistrictId = districtId },
            cancellationToken: cancellationToken);

        return await connection.QuerySingleOrDefaultAsync<int?>(command) is not null;
    }

    public async Task<bool> SalespersonExistsAsync(
        int salespersonId,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
                           SELECT 1
                           FROM dbo.Salesperson
                           WHERE SalespersonId = @SalespersonId;
                           """;

        await using var connection = await OpenConnectionAsync(cancellationToken);

        var command = new CommandDefinition(
            sql,
            new { SalespersonId = salespersonId },
            cancellationToken: cancellationToken);

        return await connection.QuerySingleOrDefaultAsync<int?>(command) is not null;
    }

    public async Task<Application.DomainModels.DistrictSalesperson?> GetSalespersonAssignmentAsync(
        int districtId,
        int salespersonId,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
                           SELECT
                               DistrictId,
                               SalespersonId,
                               Role
                           FROM dbo.DistrictSalesperson
                           WHERE DistrictId = @DistrictId
                             AND SalespersonId = @SalespersonId;
                           """;

        await using var connection = await OpenConnectionAsync(cancellationToken);

        var command = new CommandDefinition(
            sql,
            new
            {
                DistrictId = districtId,
                SalespersonId = salespersonId
            },
            cancellationToken: cancellationToken);

        return await connection.QuerySingleOrDefaultAsync<Application.DomainModels.DistrictSalesperson>(
            command);
    }

    public async Task AssignSalespersonAsync(
        int districtId,
        int salespersonId,
        SalespersonRole role,
        CancellationToken cancellationToken = default)
    {
        const string demotePrimarySql = """
                                        UPDATE dbo.DistrictSalesperson
                                        SET Role = 'Secondary'
                                        WHERE DistrictId = @DistrictId
                                          AND Role = 'Primary'
                                          AND SalespersonId <> @SalespersonId;
                                        """;

        const string updateExistingSql = """
                                         UPDATE dbo.DistrictSalesperson
                                         SET Role = @Role
                                         WHERE DistrictId = @DistrictId
                                           AND SalespersonId = @SalespersonId;
                                         """;

        const string insertSql = """
                                 INSERT INTO dbo.DistrictSalesperson
                                 (
                                     DistrictId,
                                     SalespersonId,
                                     Role
                                 )
                                 VALUES
                                 (
                                     @DistrictId,
                                     @SalespersonId,
                                     @Role
                                 );
                                 """;

        await using var connection = await OpenConnectionAsync(cancellationToken);

        await using var transaction =
            await connection.BeginTransactionAsync(cancellationToken);

        try
        {
            if (role == SalespersonRole.Primary)
            {
                await connection.ExecuteAsync(
                    new CommandDefinition(
                        demotePrimarySql,
                        new
                        {
                            DistrictId = districtId,
                            SalespersonId = salespersonId
                        },
                        transaction,
                        cancellationToken: cancellationToken));
            }

            var rowsAffected = await connection.ExecuteAsync(
                new CommandDefinition(
                    updateExistingSql,
                    new
                    {
                        DistrictId = districtId,
                        SalespersonId = salespersonId,
                        Role = role.ToString()
                    },
                    transaction,
                    cancellationToken: cancellationToken));

            if (rowsAffected == 0)
            {
                await connection.ExecuteAsync(
                    new CommandDefinition(
                        insertSql,
                        new
                        {
                            DistrictId = districtId,
                            SalespersonId = salespersonId,
                            Role = role.ToString()
                        },
                        transaction,
                        cancellationToken: cancellationToken));
            }

            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
    
    public async Task RemoveSalespersonAsync(
        int districtId,
        int salespersonId,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
                           DELETE FROM dbo.DistrictSalesperson
                           WHERE DistrictId = @DistrictId
                             AND SalespersonId = @SalespersonId;
                           """;

        await using var connection = await OpenConnectionAsync(cancellationToken);

        await connection.ExecuteAsync(
            new CommandDefinition(
                sql,
                new
                {
                    DistrictId = districtId,
                    SalespersonId = salespersonId
                },
                cancellationToken: cancellationToken));
    }
    
    private async Task<SqlConnection> OpenConnectionAsync(
        CancellationToken cancellationToken)
    {
        var connection = connectionFactory.CreateConnection();

        await connection.OpenAsync(cancellationToken);

        return connection;
    }
}