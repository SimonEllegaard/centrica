using System.Net;
using System.Net.Http.Json;

namespace SalesManagement.EndToEndTests;

public class DistrictsApiTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task GetDistricts_ReturnsSeededDistricts()
    {
        var response = await _client.GetAsync("/api/districts");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var districts =
            await response.Content.ReadFromJsonAsync<List<DistrictResponse>>();

        Assert.NotNull(districts);
        Assert.Equal(4, districts.Count);

        Assert.Contains(
            districts,
            district => district.Name == "North Denmark");

        Assert.Contains(
            districts,
            district => district.Name == "Eastern Denmark");
    }
    
    [Fact]
    public async Task GetDistrict_ReturnsDistrictDetails()
    {
        var response = await _client.GetAsync("/api/districts/1");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var district =
            await response.Content.ReadFromJsonAsync<DistrictDetailsResponse>();

        Assert.NotNull(district);
        Assert.Equal("North Denmark", district.Name);

        Assert.Equal(2, district.Salespersons.Count);
        Assert.Equal(3, district.Stores.Count);

        var primary = Assert.Single(
            district.Salespersons,
            salesperson => salesperson.Role == "Primary");

        Assert.Equal("Alice Jensen", primary.Name);
    }
    
    [Fact]
    public async Task GetDistrict_WhenDistrictDoesNotExist_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/districts/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private class DistrictResponse
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
    }
    
    private sealed class DistrictDetailsResponse
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public List<SalespersonResponse> Salespersons { get; init; } = [];
        public List<StoreResponse> Stores { get; init; } = [];
    }

    private sealed class SalespersonResponse
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string Role { get; init; } = string.Empty;
    }

    private sealed class StoreResponse
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
    }
}