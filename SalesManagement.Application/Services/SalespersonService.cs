using SalesManagement.Application.Dtos;
using SalesManagement.Application.Interfaces;

namespace SalesManagement.Application.Services;

public class SalespersonService(
    ISalespersonRepository salespersonRepository) : ISalespersonService
{
    public Task<IReadOnlyList<Salesperson>> GetSalespeopleAsync(
        CancellationToken cancellationToken)
    {
        return salespersonRepository.GetAllAsync(cancellationToken);
    }
}