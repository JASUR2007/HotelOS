namespace HotelOS.HousekeepingService.DTOs;

/// <summary>Represents a cleaning task in the housekeeping queue.</summary>
/// <param name="RoomId">Hotel room identifier. Example: 1.</param>
/// <param name="RoomNumber">Hotel room number. Example: "101".</param>
/// <param name="Status">Current cleaning status. Example: "Pending".</param>
/// <param name="AssignedTo">Name of the assigned worker. Example: "Alice Smith".</param>
public sealed record CleaningTaskDto(int RoomId, string RoomNumber, string Status, string AssignedTo);

/// <summary>Request payload for updating a cleaning task's status.</summary>
/// <param name="TaskId">Identifier of the task to update. Example: 1.</param>
/// <param name="Status">New status value. Example: "InProgress".</param>
/// <param name="AssignedTo">Name of the worker assigned to the task. Example: "Alice Smith".</param>
public sealed record UpdateCleaningStatusDto(int TaskId, string Status, string AssignedTo);

/// <summary>Request payload for creating a new cleaning task.</summary>
/// <param name="RoomNumber">Room number to clean. Example: "101".</param>
/// <param name="AssignedTo">Worker name to assign. Example: "Alice Smith".</param>
/// <param name="Priority">Task priority level. Example: "High".</param>
public sealed record CreateCleaningTaskDto(string RoomNumber, string AssignedTo, string Priority);

/// <summary>Request payload for partially updating a cleaning task.</summary>
/// <param name="Status">Optional new status. Example: "Completed".</param>
/// <param name="AssignedTo">Optional reassigned worker name. Example: "Bob Johnson".</param>
public sealed record UpdateCleaningTaskDto(string? Status, string? AssignedTo);