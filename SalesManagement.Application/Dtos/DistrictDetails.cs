namespace SalesManagement.Application.Dtos;

public class DistrictDetails
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;

    public IReadOnlyList<DistrictSalesperson> Salespersons { get; init; }
        = [];

    public IReadOnlyList<Store> Stores { get; init; }
        = [];
}