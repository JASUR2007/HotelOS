namespace HotelOS.Shared.Auth;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public sealed class HasPermissionAttribute(string permission) : Attribute
{
    public string Permission { get; } = permission;
}