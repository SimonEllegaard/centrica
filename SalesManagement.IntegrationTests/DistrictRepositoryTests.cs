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
}