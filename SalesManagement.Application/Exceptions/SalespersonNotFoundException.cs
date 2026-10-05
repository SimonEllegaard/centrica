namespace SalesManagement.Application.Exceptions;

public sealed class SalespersonNotFoundException(int salespersonId)
    : Exception($"Salesperson with ID {salespersonId} was not found.");