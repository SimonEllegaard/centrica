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

    private class DistrictResponse
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
    }
}