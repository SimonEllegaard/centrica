using SalesManagement.Application.DomainModels.Enums;

namespace SalesManagement.Application.Dtos;

public sealed class AssignSalespersonRequest
{
    public SalespersonRole Role { get; init; }
}