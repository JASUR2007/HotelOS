namespace HotelOS.HousekeepingService.DTOs;

public sealed record CleaningTaskDto(int Id, int RoomId, string RoomNumber, string Status, string AssignedTo, string Priority);

public sealed record UpdateCleaningStatusDto(int TaskId, string Status, string AssignedTo);

public sealed record CreateCleaningTaskDto(string RoomNumber, string AssignedTo, string Priority);

public sealed record UpdateCleaningTaskDto(string? Status, string? AssignedTo);