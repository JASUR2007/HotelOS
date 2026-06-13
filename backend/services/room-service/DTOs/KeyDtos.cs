namespace HotelOS.RoomService.DTOs;

public sealed record IssueKeyDto(int BranchId, int RoomId, string RoomNumber, string IssuedTo, string IssuedBy);

public sealed record ReturnKeyDto(int KeyId);

public sealed record RoomKeyDto(int Id, int BranchId, int RoomId, string RoomNumber, string KeyType, string Status, string? IssuedTo, string? IssuedBy, string? IssuedAt, string? ReturnedAt, string CreatedAt);

public sealed record CreateMasterKeyDto(string Name, string Description, string AccessScope);

public sealed record MasterKeyDto(int Id, string Name, string Description, string AccessScope, string Status, string? IssuedTo, string? IssuedAt, string? ReturnedAt, string CreatedAt);

public sealed record IssueMasterKeyDto(string IssuedTo);

public sealed record ReturnMasterKeyDto;
