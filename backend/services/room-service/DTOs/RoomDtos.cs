namespace HotelOS.RoomService.DTOs;

public sealed record RoomDto(int Id, int BranchId, string RoomNumber, string Type, string Status, decimal PricePerNight, int Floor, string Description, int GuestCapacity, string MainImage, string[] Images, string[] Amenities);

public sealed record CreateRoomDto(int BranchId, string RoomNumber, string Type, int Floor, decimal PricePerNight, int GuestCapacity, string Description, string MainImage, string[] Images, string[] AmenityIds, string? Status = null);

public sealed record UpdateRoomDto(int BranchId, string RoomNumber, string Type, string Status, decimal PricePerNight, int Floor, string Description, int GuestCapacity, string MainImage, string[] Images, string[] Amenities);

public sealed record PatchRoomStatusDto(string Status);

public sealed record UpdateOrderStatusDto(string Status);

public sealed record RoomOverviewDto(int Id, string RoomNumber, string Status, string GuestName, string Housekeeping);

public sealed record MenuItemDto(int Id, string Name, decimal Price);

public sealed record CreateMenuItemDto(string Name, decimal Price);

public sealed record UpdateMenuItemDto(string Name, decimal Price);

public sealed record OrderDto(int Id, string RoomNumber, string GuestName, string Status, decimal Total);

public sealed record CreateOrderDto(string RoomNumber, string GuestName, string[] Items);

public sealed record AmenityDto(int Id, string Name, string IconUrl, string Description);

public sealed record UploadImageResponse(string Url);