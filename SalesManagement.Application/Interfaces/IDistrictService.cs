using SalesManagement.Application.DomainModels;
using SalesManagement.Application.Dtos;

namespace SalesManagement.Application.Interfaces;

public interface IDistrictService
{
    public Task<IReadOnlyList<District>> GetDistrictsAsync(
        CancellationToken cancellationToken = default);

    public Task<DistrictDetails?> GetDistrictDetailsAsync(
        int districtId,
        CancellationToken cancellationToken = default);
}