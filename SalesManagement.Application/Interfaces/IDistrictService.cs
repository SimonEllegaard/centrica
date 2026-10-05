using SalesManagement.Application.DomainModels;
using SalesManagement.Application.DomainModels.Enums;
using SalesManagement.Application.Dtos;

namespace SalesManagement.Application.Interfaces;

public interface IDistrictService
{
    Task<IReadOnlyList<District>> GetDistrictsAsync(
        CancellationToken cancellationToken = default);

    Task<DistrictDetails?> GetDistrictDetailsAsync(
        int districtId,
        CancellationToken cancellationToken = default);

    Task AssignSalespersonAsync(
        int districtId,
        int salespersonId,
        SalespersonRole role,
        CancellationToken cancellationToken = default);

    Task RemoveSalespersonAsync(
        int districtId,
        int salespersonId,
        CancellationToken cancellationToken = default);
}