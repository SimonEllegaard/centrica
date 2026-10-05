namespace SalesManagement.Application.Exceptions;

public sealed class DistrictNotFoundException(int districtId)
    : Exception($"District with ID {districtId} was not found.");