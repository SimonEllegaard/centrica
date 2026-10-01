using SalesManagement.Data.DomainModel.Enums;

namespace SalesManagement.Data.DomainModel;

public class DistrictSalesperson
{
    public int DistrictId { get; init; }
    public int SalespersonId { get; init; }
    public SalespersonRole Role { get; init; }
}