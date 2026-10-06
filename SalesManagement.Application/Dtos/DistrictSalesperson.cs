using SalesManagement.Application.DomainModels.Enums;

namespace SalesManagement.Application.Dtos;

public class DistrictSalesperson
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public SalespersonRole Role { get; init; }
}