namespace SalesManagement.Application.Dtos;

public class DistrictDetails
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;

    public IReadOnlyList<DistrictSalespersonDto> Salespersons { get; init; }
        = [];

    public IReadOnlyList<StoreDto> Stores { get; init; }
        = [];
}