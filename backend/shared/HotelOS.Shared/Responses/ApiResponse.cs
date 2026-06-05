namespace HotelOS.Shared.Responses;

public sealed record ApiResponse(bool Success, string Message, object? Data = null);