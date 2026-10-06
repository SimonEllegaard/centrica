using SalesManagement.Application.Dtos;
using SalesManagement.Application.DomainModels.Enums;
using SalesManagement.Application.DomainModels;
using SalesManagement.Application.Interfaces;
using DistrictSalesperson = SalesManagement.Application.DomainModels.DistrictSalesperson;
using Salesperson = SalesManagement.Application.DomainModels.Salesperson;

namespace SalesManagement.UnitTests;

internal class FakeDistrictRepository : IDistrictRepository
{
    public List<District> Districts { get; } = [];
    public List<Salesperson> Salespersons { get; } = [];
    public List<DistrictSalesperson> Assignments { get; } = [];

    public int AssignSalespersonCallCount { get; private set; }
    public int RemoveSalespersonCallCount { get; private set; }

    public Task<IReadOnlyList<District>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyList<District>>(Districts);
    }

    public Task<DistrictDetails?> GetDetailsAsync(
        int districtId,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DistrictExistsAsync(
        int districtId,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(
            Districts.Any(x => x.Id == districtId));
    }

    public Task<bool> SalespersonExistsAsync(
        int salespersonId,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(
            Salespersons.Any(x => x.Id == salespersonId));
    }

    public Task<DistrictSalesperson?> GetSalespersonAssignmentAsync(
        int districtId,
        int salespersonId,
        CancellationToken cancellationToken = default)
    {
        var assignment = Assignments.SingleOrDefault(
            x => x.DistrictId == districtId
              && x.SalespersonId == salespersonId);

        return Task.FromResult(assignment);
    }

    public Task AssignSalespersonAsync(
        int districtId,
        int salespersonId,
        SalespersonRole role,
        CancellationToken cancellationToken = default)
    {
        AssignSalespersonCallCount++;

        var existing = Assignments.SingleOrDefault(
            x => x.DistrictId == districtId
              && x.SalespersonId == salespersonId);

        if (existing is not null)
        {
            Assignments.Remove(existing);
        }

        Assignments.Add(new DistrictSalesperson
        {
            DistrictId = districtId,
            SalespersonId = salespersonId,
            Role = role
        });

        return Task.CompletedTask;
    }

    public Task RemoveSalespersonAsync(
        int districtId,
        int salespersonId,
        CancellationToken cancellationToken = default)
    {
        RemoveSalespersonCallCount++;

        var existing = Assignments.SingleOrDefault(
            x => x.DistrictId == districtId
              && x.SalespersonId == salespersonId);

        if (existing is not null)
        {
            Assignments.Remove(existing);
        }

        return Task.CompletedTask;
    }
}