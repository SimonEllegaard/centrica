using SalesManagement.Application.Dtos;

namespace SalesManagement.Application.Interfaces;

public interface ISalespersonService
{
    Task<IReadOnlyList<Salesperson>> GetSalespeopleAsync(
        CancellationToken cancellationToken);
}