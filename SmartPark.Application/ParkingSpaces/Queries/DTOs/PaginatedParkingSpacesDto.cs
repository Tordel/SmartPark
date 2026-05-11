namespace SmartPark.Application.ParkingSpaces.Queries.DTOs;

public record PaginatedParkingSpacesDto(
    List<ParkingSpaceDto> Items,
    int PageNumber,
    int PageSize,
    int TotalCount,
    int TotalPages,
    bool HasPreviousPage,
    bool HasNextPage
    );