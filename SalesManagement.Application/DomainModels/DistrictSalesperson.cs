using SalesManagement.Application.DomainModels.Enums;

namespace SalesManagement.Application.DomainModels;

public class DistrictSalesperson
{
    public int DistrictId { get; init; }
    public int SalespersonId { get; init; }
    public SalespersonRole Role { get; init; }
}