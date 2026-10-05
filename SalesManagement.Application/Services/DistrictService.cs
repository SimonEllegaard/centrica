using SalesManagement.Application.DomainModels.Enums;
using SalesManagement.Application.Exceptions;

namespace SalesManagement.Application.Services;

using Dtos;
using Interfaces;
using DomainModels;

public class DistrictService(IDistrictRepository districtRepository) : IDistrictService
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

    public async Task AssignSalespersonAsync(
        int districtId,
        int salespersonId,
        SalespersonRole role,
        CancellationToken cancellationToken = default)
    {
        if (!await districtRepository.DistrictExistsAsync(
                districtId,
                cancellationToken))
        {
            throw new DistrictNotFoundException(districtId);
        }

        if (!await districtRepository.SalespersonExistsAsync(
                salespersonId,
                cancellationToken))
        {
            throw new SalespersonNotFoundException(salespersonId);
        }

        await districtRepository.AssignSalespersonAsync(
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
        if (!await districtRepository.DistrictExistsAsync(
                districtId,
                cancellationToken))
        {
            throw new DistrictNotFoundException(districtId);
        }

        if (!await districtRepository.SalespersonExistsAsync(
                salespersonId,
                cancellationToken))
        {
            throw new SalespersonNotFoundException(salespersonId);
        }

        var assignment =
            await districtRepository.GetSalespersonAssignmentAsync(
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

        await districtRepository.RemoveSalespersonAsync(
            districtId,
            salespersonId,
            cancellationToken);
    }
}