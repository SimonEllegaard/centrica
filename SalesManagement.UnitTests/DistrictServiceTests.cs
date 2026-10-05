using SalesManagement.Application.DomainModels.Enums;
using SalesManagement.Application.DomainModels;
using SalesManagement.Application.Exceptions;
using SalesManagement.Application.Services;
using Xunit;

namespace SalesManagement.UnitTests;

public class DistrictServiceTests
{
    [Fact]
    public async Task AssignSalesperson_UnknownDistrict_Throws()
    {
        var repository = CreateRepository();
        var service = new DistrictService(repository);

        var exception = await Assert.ThrowsAsync<DistrictNotFoundException>(
            () => service.AssignSalespersonAsync(
                999,
                1,
                SalespersonRole.Secondary));

        Assert.Equal(
            "District with ID 999 was not found.",
            exception.Message);
    }

    [Fact]
    public async Task AssignSalesperson_UnknownSalesperson_Throws()
    {
        var repository = CreateRepository();

        repository.Districts.Add(new District
        {
            Id = 1,
            Name = "North Denmark"
        });

        var service = new DistrictService(repository);

        var exception = await Assert.ThrowsAsync<SalespersonNotFoundException>(
            () => service.AssignSalespersonAsync(
                1,
                999,
                SalespersonRole.Secondary));

        Assert.Equal(
            "Salesperson with ID 999 was not found.",
            exception.Message);
    }

    [Fact]
    public async Task AssignSalesperson_ValidAssignment_DelegatesToRepository()
    {
        var repository = CreateRepository();

        repository.Districts.Add(new District
        {
            Id = 1,
            Name = "North Denmark"
        });

        repository.Salespersons.Add(new Salesperson
        {
            Id = 2,
            Name = "Bob Hansen"
        });

        var service = new DistrictService(repository);

        await service.AssignSalespersonAsync(
            1,
            2,
            SalespersonRole.Secondary);

        Assert.Equal(
            1,
            repository.AssignSalespersonCallCount);

        var assignment = Assert.Single(repository.Assignments);

        Assert.Equal(1, assignment.DistrictId);
        Assert.Equal(2, assignment.SalespersonId);
        Assert.Equal(SalespersonRole.Secondary, assignment.Role);
    }

    [Fact]
    public async Task RemoveSalesperson_Secondary_DelegatesToRepository()
    {
        var repository = CreateRepository();

        repository.Districts.Add(new District
        {
            Id = 1,
            Name = "North Denmark"
        });

        repository.Salespersons.Add(new Salesperson
        {
            Id = 2,
            Name = "Bob Hansen"
        });

        repository.Assignments.Add(new DistrictSalesperson
        {
            DistrictId = 1,
            SalespersonId = 2,
            Role = SalespersonRole.Secondary
        });

        var service = new DistrictService(repository);

        await service.RemoveSalespersonAsync(1, 2);

        Assert.Equal(
            1,
            repository.RemoveSalespersonCallCount);

        Assert.Empty(repository.Assignments);
    }

    [Fact]
    public async Task RemoveSalesperson_Primary_ThrowsConflictException()
    {
        var repository = CreateRepository();

        repository.Districts.Add(new District
        {
            Id = 1,
            Name = "North Denmark"
        });

        repository.Salespersons.Add(new Salesperson
        {
            Id = 1,
            Name = "Alice Jensen"
        });

        repository.Assignments.Add(new DistrictSalesperson
        {
            DistrictId = 1,
            SalespersonId = 1,
            Role = SalespersonRole.Primary
        });

        var service = new DistrictService(repository);

        var exception =
            await Assert.ThrowsAsync<PrimarySalespersonRequiredException>(
                () => service.RemoveSalespersonAsync(1, 1));

        Assert.Equal(
            "District with ID 1 must have a primary salesperson.",
            exception.Message);

        Assert.Equal(
            0,
            repository.RemoveSalespersonCallCount);
    }

    [Fact]
    public async Task RemoveSalesperson_NotAssigned_DoesNothing()
    {
        var repository = CreateRepository();

        repository.Districts.Add(new District
        {
            Id = 1,
            Name = "North Denmark"
        });

        repository.Salespersons.Add(new Salesperson
        {
            Id = 2,
            Name = "Bob Hansen"
        });

        var service = new DistrictService(repository);

        await service.RemoveSalespersonAsync(1, 2);

        Assert.Equal(
            0,
            repository.RemoveSalespersonCallCount);
    }

    private static FakeDistrictRepository CreateRepository()
    {
        return new FakeDistrictRepository();
    }
}