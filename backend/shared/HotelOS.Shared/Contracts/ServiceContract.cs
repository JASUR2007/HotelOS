namespace HotelOS.Shared.Contracts;

public sealed record ServiceContract(string Name, string BaseUrl, bool RequiresAuth = true);