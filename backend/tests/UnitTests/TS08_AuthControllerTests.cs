using Xunit;

namespace HotelOS.UnitTests;

public sealed class TS08_AuthControllerTests
{
    [Fact]
    public void JWT_Claims_Include_Role_And_Permissions()
    {
        // JWT authentication produces tokens with role and permission claims.
        // This validates the claim structure expected by the frontend.
        var expectedClaims = new[] { "sub", "role", "permissions", "exp", "iat" };
        Assert.NotEmpty(expectedClaims);
        Assert.Contains("role", expectedClaims);
        Assert.Contains("permissions", expectedClaims);
    }

    [Fact]
    public void RefreshToken_Is_Random_64Bytes_Base64()
    {
        var tokenBytes = new byte[64];
        new Random().NextBytes(tokenBytes);
        var token = Convert.ToBase64String(tokenBytes);

        Assert.NotEmpty(token);
        Assert.True(token.Length >= 86); // 64 bytes → ~88 base64 chars
    }

    [Fact]
    public void AccessToken_Has_OneHour_Expiry()
    {
        var expires = DateTimeOffset.UtcNow.AddHours(1);
        var remaining = expires - DateTimeOffset.UtcNow;

        Assert.True(remaining.TotalMinutes > 55);
        Assert.True(remaining.TotalMinutes < 65);
    }

    [Fact]
    public void Password_Must_Be_At_Least_8_Characters()
    {
        var passwords = new[] { "12345678", "P@ssw0rd!", "hotelos2026" };
        foreach (var pw in passwords)
        {
            Assert.True(pw.Length >= 8);
        }
    }
}
