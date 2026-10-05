using SalesManagement.Application.DomainModels.Enums;
using Xunit;

namespace SalesManagement.IntegrationTests;

public class DistrictRepositoryTests(DatabaseFixture fixture) : IClassFixture<DatabaseFixture>
{
    [Fact]
    public async Task GetAllAsync_ReturnsSeededDistricts()
    {
        var districts =
            await fixture.DistrictRepository.GetAllAsync();

        Assert.NotEmpty(districts);

        Assert.Contains(
            districts,
            district => district.Name == "North Denmark");

        Assert.Contains(
            districts,
            district => district.Name == "Southern Denmark");

        Assert.Contains(
            districts,
            district => district.Name == "Central Denmark");

        Assert.Contains(
            districts,
            district => district.Name == "Eastern Denmark");
    }
    
    [Fact]
    public async Task GetDetailsAsync_ReturnsDistrictWithSalespersonsAndStores()
    {
        var districts =
            await fixture.DistrictRepository.GetAllAsync();

        var northDenmark = Assert.Single(districts, district => district.Name == "North Denmark");

        var district =
            await fixture.DistrictRepository.GetDetailsAsync(
                northDenmark.Id);

        Assert.NotNull(district);

        Assert.Equal(
            "North Denmark",
            district.Name);

        Assert.Equal(
            2,
            district.Salespersons.Count);

        Assert.Equal(
            3,
            district.Stores.Count);
    }
    
    [Fact]
    public async Task GetDetailsAsync_ReturnsCorrectSalespersonRoles()
    {
        var districts =
            await fixture.DistrictRepository.GetAllAsync();

        var northDenmark = Assert.Single(districts, x => x.Name == "North Denmark");

        var district =
            await fixture.DistrictRepository.GetDetailsAsync(
                northDenmark.Id);

        Assert.NotNull(district);

        var alice = Assert.Single(
            district.Salespersons,
            salesperson => salesperson.Name == "Alice Jensen");

        var bob = Assert.Single(
            district.Salespersons,
            salesperson => salesperson.Name == "Bob Hansen");

        Assert.Equal(
            SalespersonRole.Primary,
            alice.Role);

        Assert.Equal(
            SalespersonRole.Secondary,
            bob.Role);
    }
    
    [Fact]
    public async Task GetDetailsAsync_ReturnsStoresBelongingToDistrict()
    {
        var districts =
            await fixture.DistrictRepository.GetAllAsync();

        var northDenmark = Assert.Single(districts, x => x.Name == "North Denmark");

        var district =
            await fixture.DistrictRepository.GetDetailsAsync(
                northDenmark.Id);

        Assert.NotNull(district);

        var storeNames = district.Stores
            .Select(store => store.Name)
            .ToHashSet();

        Assert.Contains("Aalborg Store", storeNames);
        Assert.Contains("Frederikshavn Store", storeNames);
        Assert.Contains("Thisted Store", storeNames);
    }
    
    [Fact]
    public async Task GetDetailsAsync_ForUnknownDistrict_ReturnsNull()
    {
        var district =
            await fixture.DistrictRepository.GetDetailsAsync(
                int.MaxValue);

        Assert.Null(district);
    }
    
    [Fact]
    public async Task AssignSalespersonAsync_AddsSecondarySalesperson()
    {
        const int districtId = 3;     // Central Denmark
        const int salespersonId = 2; // Bob Hansen

        try
        {
            await fixture.DistrictRepository.AssignSalespersonAsync(
                districtId,
                salespersonId,
                SalespersonRole.Secondary);

            var district = await fixture.DistrictRepository.GetDetailsAsync(districtId);

            var assignment = Assert.Single(district!.Salespersons, x => x.Id == salespersonId);

            Assert.Equal(SalespersonRole.Secondary, assignment.Role);
        }
        finally
        {
            await fixture.DistrictRepository.RemoveSalespersonAsync(
                districtId,
                salespersonId);
        }
    }
    
    [Fact]
    public async Task AssignSalespersonAsync_ReplacesPrimarySalesperson()
    {
        const int districtId = 4;     // Eastern Denmark
        const int salespersonId = 2; // Bob Hansen
        const int originalPrimaryId = 5; // Erik Andersen

        try
        {
            await fixture.DistrictRepository.AssignSalespersonAsync(
                districtId,
                salespersonId,
                SalespersonRole.Primary);

            var district = await fixture.DistrictRepository.GetDetailsAsync(districtId);

            var bob = Assert.Single(district!.Salespersons, x => x.Id == salespersonId);

            var erik = Assert.Single(district.Salespersons, x => x.Id == originalPrimaryId);

            Assert.Equal(SalespersonRole.Primary, bob.Role);
            Assert.Equal(SalespersonRole.Secondary, erik.Role);

            Assert.Single(district.Salespersons, x => x.Role == SalespersonRole.Primary);
        }
        finally
        {
            // Restore the seeded state.
            await fixture.DistrictRepository.AssignSalespersonAsync(
                districtId,
                originalPrimaryId,
                SalespersonRole.Primary);

            await fixture.DistrictRepository.RemoveSalespersonAsync(
                districtId,
                salespersonId);
        }
    }
    
    [Fact]
    public async Task RemoveSalespersonAsync_RemovesSecondarySalesperson()
    {
        const int districtId = 4;     // Eastern Denmark
        const int salespersonId = 2; // Bob Hansen

        try
        {
            await fixture.DistrictRepository.RemoveSalespersonAsync(
                districtId,
                salespersonId);

            var district = await fixture.DistrictRepository.GetDetailsAsync(districtId);

            Assert.DoesNotContain(
                district!.Salespersons,
                salesperson => salesperson.Id == salespersonId);
        }
        finally
        {
            // Restore seeded state.
            await fixture.DistrictRepository.AssignSalespersonAsync(
                districtId,
                salespersonId,
                SalespersonRole.Secondary);
        }
    }
    
    [Fact]
    public async Task AssignSalespersonAsync_WhenInsertFails_RollsBackPrimaryChange()
    {
        const int districtId = 4;       // Eastern Denmark
        const int invalidSalespersonId = int.MaxValue;
        const int originalPrimaryId = 5; // Erik Andersen

        await Assert.ThrowsAnyAsync<Exception>(() =>
            fixture.DistrictRepository.AssignSalespersonAsync(
                districtId,
                invalidSalespersonId,
                SalespersonRole.Primary));

        var district = await fixture.DistrictRepository.GetDetailsAsync(districtId);

        var primary = Assert.Single(district!.Salespersons, x => x.Role == SalespersonRole.Primary);

        Assert.Equal(originalPrimaryId, primary.Id);
    }
}