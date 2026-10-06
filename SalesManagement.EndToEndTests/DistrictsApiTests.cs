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
    
    [Fact]
    public async Task AssignSalesperson_AsSecondary_ReturnsNoContent()
    {
        const int districtId = 3;     // Central Denmark
        const int salespersonId = 2; // Bob Hansen

        try
        {
            var response = await _client.PutAsJsonAsync(
                $"/api/districts/{districtId}/salespersons/{salespersonId}",
                new { role = "Secondary" });

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            var district = await GetDistrictAsync(districtId);

            var salesperson = Assert.Single(
                district.Salespersons,
                x => x.Id == salespersonId);

            Assert.Equal("Secondary", salesperson.Role);
        }
        finally
        {
            await _client.DeleteAsync(
                $"/api/districts/{districtId}/salespersons/{salespersonId}");
        }
    }
    
    [Fact]
    public async Task AssignSalesperson_AsPrimary_ReplacesExistingPrimary()
    {
        const int districtId = 4;     // Eastern Denmark
        const int salespersonId = 2; // Bob Hansen
        const int originalPrimaryId = 5; // Erik Andersen

        try
        {
            var response = await _client.PutAsJsonAsync(
                $"/api/districts/{districtId}/salespersons/{salespersonId}",
                new { role = "Primary" });

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            var district = await GetDistrictAsync(districtId);

            var bob = Assert.Single(
                district.Salespersons,
                x => x.Id == salespersonId);

            var erik = Assert.Single(
                district.Salespersons,
                x => x.Id == originalPrimaryId);

            Assert.Equal("Primary", bob.Role);
            Assert.Equal("Secondary", erik.Role);

            Assert.Single(
                district.Salespersons,
                x => x.Role == "Primary");
        }
        finally
        {
            // Restore seeded state.
            var restoreResponse = await _client.PutAsJsonAsync(
                $"/api/districts/{districtId}/salespersons/{originalPrimaryId}",
                new { role = "Primary" });

            restoreResponse.EnsureSuccessStatusCode();

            await _client.DeleteAsync(
                $"/api/districts/{districtId}/salespersons/{salespersonId}");
        }
    }
    
    [Fact]
    public async Task DeleteSalesperson_RemovesSecondary()
    {
        const int districtId = 4;
        const int salespersonId = 2;

        try
        {
            var response = await _client.DeleteAsync(
                $"/api/districts/{districtId}/salespersons/{salespersonId}");

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            var district = await GetDistrictAsync(districtId);

            Assert.DoesNotContain(
                district.Salespersons,
                x => x.Id == salespersonId);
        }
        finally
        {
            await _client.PutAsJsonAsync(
                $"/api/districts/{districtId}/salespersons/{salespersonId}",
                new { role = "Secondary" });
        }
    }
    
    [Fact]
    public async Task DeleteSalesperson_WhenPrimary_ReturnsConflict()
    {
        const int districtId = 1;     // North Denmark
        const int salespersonId = 1; // Alice Jensen

        var response = await _client.DeleteAsync(
            $"/api/districts/{districtId}/salespersons/{salespersonId}");

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);

        var error =
            await response.Content.ReadFromJsonAsync<ErrorResponse>();

        Assert.NotNull(error);
        Assert.Equal("PrimarySalespersonRequired", error.Code);

        // Verify Alice is still primary.
        var district = await GetDistrictAsync(districtId);

        var alice = Assert.Single(
            district.Salespersons,
            x => x.Id == salespersonId);

        Assert.Equal("Primary", alice.Role);
    }
    
    [Fact]
    public async Task AssignSalesperson_WhenDistrictDoesNotExist_ReturnsNotFound()
    {
        var response = await _client.PutAsJsonAsync(
            "/api/districts/999999/salespersons/1",
            new { role = "Secondary" });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    [Fact]
    public async Task AssignSalesperson_WhenSalespersonDoesNotExist_ReturnsNotFound()
    {
        var response = await _client.PutAsJsonAsync(
            "/api/districts/1/salespersons/999999",
            new { role = "Secondary" });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    [Fact]
    public async Task DeleteSalesperson_WhenDistrictDoesNotExist_ReturnsNotFound()
    {
        var response = await _client.DeleteAsync(
            "/api/districts/999999/salespersons/1");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private class DistrictResponse
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
    }
    
    private class DistrictDetailsResponse
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public List<SalespersonResponse> Salespersons { get; init; } = [];
        public List<StoreResponse> Stores { get; init; } = [];
    }

    private class SalespersonResponse
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string Role { get; init; } = string.Empty;
    }

    private class StoreResponse
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
    }
    
    private async Task<DistrictDetailsResponse> GetDistrictAsync(int districtId)
    {
        var response = await _client.GetAsync($"/api/districts/{districtId}");

        response.EnsureSuccessStatusCode();

        return (await response.Content
            .ReadFromJsonAsync<DistrictDetailsResponse>())!;
    }
    
    private class ErrorResponse
    {
        public string Code { get; init; } = string.Empty;
        public string Message { get; init; } = string.Empty;
    }
}