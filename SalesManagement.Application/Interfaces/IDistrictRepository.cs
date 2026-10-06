using SalesManagement.Application.Dtos;
using SalesManagement.Application.DomainModels;
using SalesManagement.Application.DomainModels.Enums;
using DistrictSalesperson = SalesManagement.Application.DomainModels.DistrictSalesperson;

namespace SalesManagement.Application.Interfaces;

public interface IDistrictRepository
{
    Task<IReadOnlyList<District>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<DistrictDetails?> GetDetailsAsync(
        int districtId,
        CancellationToken cancellationToken = default);

    Task<bool> DistrictExistsAsync(
        int districtId,
        CancellationToken cancellationToken = default);

    Task<bool> SalespersonExistsAsync(
        int salespersonId,
        CancellationToken cancellationToken = default);

    Task<DistrictSalesperson?> GetSalespersonAssignmentAsync(
        int districtId,
        int salespersonId,
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