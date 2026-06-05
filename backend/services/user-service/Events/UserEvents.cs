namespace HotelOS.UserService.Events;

public sealed record UserLoggedInEvent(int UserId, string Email, DateTimeOffset OccurredAt);
public sealed record RoleUpdatedEvent(int UserId, string RoleName, DateTimeOffset OccurredAt);