using SalesManagement.Application.Dtos;

namespace SalesManagement.Application.Interfaces;

public interface ISalespersonRepository
{
    Task<IReadOnlyList<Salesperson>> GetAllAsync(
        CancellationToken cancellationToken);
}