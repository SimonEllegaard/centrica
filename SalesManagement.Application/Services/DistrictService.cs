namespace SalesManagement.Application.Services;

using Dtos;
using Interfaces;
using DomainModels;

public sealed class DistrictService(IDistrictRepository districtRepository) : IDistrictService
{
    public Task<IReadOnlyList<District>> GetDistrictsAsync(
        CancellationToken cancellationToken = default)
    {
        return districtRepository.GetAllAsync(cancellationToken);
    }

    public Task<DistrictDetails?> GetDistrictDetailsAsync(
        int districtId,
        CancellationToken cancellationToken = default)
    {
        return districtRepository.GetDetailsAsync(
            districtId,
            cancellationToken);
    }
}