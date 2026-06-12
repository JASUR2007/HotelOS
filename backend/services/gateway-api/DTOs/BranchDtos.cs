namespace HotelOS.GatewayApi.DTOs;

public sealed record CreateBranchDto(string Name, string Address, string City, string Country, string Phone, string Email);

public sealed record UpdateBranchDto(string Name, string Address, string City, string Country, string Phone, string Email, string Status);

public sealed record BranchDto(int Id, string Name, string Address, string City, string Country, string Phone, string Email, string Status, string CreatedAt);
