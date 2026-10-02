namespace SalesManagement.Application.Services;

using Dtos;
using Interfaces;
using DomainModels;

public sealed class DistrictService : IDistrictService
{
    private readonly IDistrictRepository _districtRepository;

    public DistrictService(IDistrictRepository districtRepository)
    {
        _districtRepository = districtRepository;
    }

    public Task<IReadOnlyList<District>> GetDistrictsAsync(
        CancellationToken cancellationToken = default)
    {
        return _districtRepository.GetAllAsync(cancellationToken);
    }

    public Task<DistrictDetails?> GetDistrictDetailsAsync(
        int districtId,
        CancellationToken cancellationToken = default)
    {
        return _districtRepository.GetDetailsAsync(
            districtId,
            cancellationToken);
    }
}