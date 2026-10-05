namespace SalesManagement.Application.Exceptions;

public sealed class PrimarySalespersonRequiredException(int districtId)
    : Exception($"District with ID {districtId} must have a primary salesperson.");