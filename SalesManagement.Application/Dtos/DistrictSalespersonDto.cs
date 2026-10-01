using SalesManagement.Data.DomainModel.Enums;

namespace SalesManagement.Application.Dtos;

public class DistrictSalespersonDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public SalespersonRole Role { get; init; }
}