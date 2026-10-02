using SalesManagement.Application.DomainModels;
using SalesManagement.Application.Dtos;

namespace SalesManagement.Application.Interfaces;

public interface IDistrictRepository
{
    Task<IReadOnlyList<District>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<DistrictDetails?> GetDetailsAsync(
        int districtId,
        CancellationToken cancellationToken = default);
}