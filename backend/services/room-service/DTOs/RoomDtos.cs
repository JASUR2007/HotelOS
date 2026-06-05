namespace HotelOS.RoomService.DTOs;

/// <summary>
/// Represents a room as returned by the API.
/// </summary>
/// <param name="Id"><example>1</example></param>
/// <param name="RoomNumber"><example>101</example></param>
/// <param name="Type"><example>Deluxe</example></param>
/// <param name="Status"><example>Available</example></param>
/// <param name="PricePerNight"><example>150.00</example></param>
/// <param name="Floor"><example>2</example></param>
/// <param name="Description"><example>Spacious room with city view</example></param>
/// <param name="GuestCapacity"><example>2</example></param>
/// <param name="MainImage"><example>/images/rooms/room-1.png</example></param>
/// <param name="Images"><example>["/images/rooms/room-1a.png","/images/rooms/room-1b.png"]</example></param>
/// <param name="Amenities"><example>["WiFi","Air Conditioning","TV"]</example></param>
public sealed record RoomDto(int Id, string RoomNumber, string Type, string Status, decimal PricePerNight, int Floor, string Description, int GuestCapacity, string MainImage, string[] Images, string[] Amenities);

/// <summary>
/// Data required to create a new room.
/// </summary>
/// <param name="RoomNumber"><example>102</example></param>
/// <param name="Type"><example>Standard</example></param>
/// <param name="Floor"><example>1</example></param>
/// <param name="PricePerNight"><example>100.00</example></param>
/// <param name="GuestCapacity"><example>2</example></param>
/// <param name="Description"><example>Cozy room with garden view</example></param>
/// <param name="MainImage"><example>/images/rooms/room-2.png</example></param>
/// <param name="Images"><example>["/images/rooms/room-2a.png"]</example></param>
/// <param name="AmenityIds"><example>["1","2","3"]</example></param>
/// <param name="Status"><example>Available</example></param>
public sealed record CreateRoomDto(string RoomNumber, string Type, int Floor, decimal PricePerNight, int GuestCapacity, string Description, string MainImage, string[] Images, string[] AmenityIds, string? Status = null);

/// <summary>
/// Data required to update an existing room.
/// </summary>
/// <param name="RoomNumber"><example>102</example></param>
/// <param name="Type"><example>Standard</example></param>
/// <param name="Status"><example>Occupied</example></param>
/// <param name="PricePerNight"><example>100.00</example></param>
/// <param name="Floor"><example>1</example></param>
/// <param name="Description"><example>Cozy room with garden view</example></param>
/// <param name="GuestCapacity"><example>2</example></param>
/// <param name="MainImage"><example>/images/rooms/room-2.png</example></param>
/// <param name="Images"><example>["/images/rooms/room-2a.png"]</example></param>
/// <param name="Amenities"><example>["WiFi","Air Conditioning"]</example></param>
public sealed record UpdateRoomDto(string RoomNumber, string Type, string Status, decimal PricePerNight, int Floor, string Description, int GuestCapacity, string MainImage, string[] Images, string[] Amenities);

/// <summary>
/// Data required to patch the status of a room.
/// </summary>
/// <param name="Status"><example>Occupied</example></param>
public sealed record PatchRoomStatusDto(string Status);

/// <summary>
/// Data required to update the status of an order.
/// </summary>
/// <param name="Status"><example>Delivered</example></param>
public sealed record UpdateOrderStatusDto(string Status);

/// <summary>
/// Represents a room overview entry displayed on the dashboard.
/// </summary>
/// <param name="Id"><example>1</example></param>
/// <param name="RoomNumber"><example>101</example></param>
/// <param name="Status"><example>Available</example></param>
/// <param name="GuestName"><example>John Doe</example></param>
/// <param name="Housekeeping"><example>Cleaning</example></param>
public sealed record RoomOverviewDto(int Id, string RoomNumber, string Status, string GuestName, string Housekeeping);

/// <summary>
/// Represents a menu item as returned by the API.
/// </summary>
/// <param name="Id"><example>1</example></param>
/// <param name="Name"><example>Club Sandwich</example></param>
/// <param name="Price"><example>12.99</example></param>
public sealed record MenuItemDto(int Id, string Name, decimal Price);

/// <summary>
/// Data required to create a new menu item.
/// </summary>
/// <param name="Name"><example>Club Sandwich</example></param>
/// <param name="Price"><example>12.99</example></param>
public sealed record CreateMenuItemDto(string Name, decimal Price);

/// <summary>
/// Data required to update an existing menu item.
/// </summary>
/// <param name="Name"><example>Club Sandwich</example></param>
/// <param name="Price"><example>12.99</example></param>
public sealed record UpdateMenuItemDto(string Name, decimal Price);

/// <summary>
/// Represents an order as returned by the API.
/// </summary>
/// <param name="Id"><example>1</example></param>
/// <param name="RoomNumber"><example>101</example></param>
/// <param name="GuestName"><example>John Doe</example></param>
/// <param name="Status"><example>Pending</example></param>
/// <param name="Total"><example>25.98</example></param>
public sealed record OrderDto(int Id, string RoomNumber, string GuestName, string Status, decimal Total);

/// <summary>
/// Data required to create a new order.
/// </summary>
/// <param name="RoomNumber"><example>101</example></param>
/// <param name="GuestName"><example>John Doe</example></param>
/// <param name="Items"><example>["Club Sandwich","French Fries"]</example></param>
public sealed record CreateOrderDto(string RoomNumber, string GuestName, string[] Items);

/// <summary>
/// Represents an amenity as returned by the API.
/// </summary>
/// <param name="Id"><example>1</example></param>
/// <param name="Name"><example>WiFi</example></param>
/// <param name="IconUrl"><example>/images/amenities/wifi.svg</example></param>
/// <param name="Description"><example>High-speed internet access</example></param>
public sealed record AmenityDto(int Id, string Name, string IconUrl, string Description);

/// <summary>
/// Response from the image upload endpoint.
/// </summary>
/// <param name="Url"><example>/images/rooms/room-9.png</example></param>
public sealed record UploadImageResponse(string Url);
