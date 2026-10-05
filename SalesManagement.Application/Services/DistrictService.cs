using SalesManagement.Application.DomainModels.Enums;
using SalesManagement.Application.Exceptions;

namespace SalesManagement.Application.Services;

using Dtos;
using Interfaces;
using DomainModels;

public class DistrictService : IDistrictService
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

    public async Task AssignSalespersonAsync(
        int districtId,
        int salespersonId,
        SalespersonRole role,
        CancellationToken cancellationToken = default)
    {
        if (!await _districtRepository.DistrictExistsAsync(
                districtId,
                cancellationToken))
        {
            throw new DistrictNotFoundException(districtId);
        }

        if (!await _districtRepository.SalespersonExistsAsync(
                salespersonId,
                cancellationToken))
        {
            throw new SalespersonNotFoundException(salespersonId);
        }

        await _districtRepository.AssignSalespersonAsync(
            districtId,
            salespersonId,
            role,
            cancellationToken);
    }

    public async Task RemoveSalespersonAsync(
        int districtId,
        int salespersonId,
        CancellationToken cancellationToken = default)
    {
        if (!await _districtRepository.DistrictExistsAsync(
                districtId,
                cancellationToken))
        {
            throw new DistrictNotFoundException(districtId);
        }

        if (!await _districtRepository.SalespersonExistsAsync(
                salespersonId,
                cancellationToken))
        {
            throw new SalespersonNotFoundException(salespersonId);
        }

        var assignment =
            await _districtRepository.GetSalespersonAssignmentAsync(
                districtId,
                salespersonId,
                cancellationToken);

        if (assignment is null)
        {
            return;
        }

        if (assignment.Role == SalespersonRole.Primary)
        {
            throw new PrimarySalespersonRequiredException(districtId);
        }

        await _districtRepository.RemoveSalespersonAsync(
            districtId,
            salespersonId,
            cancellationToken);
    }
}