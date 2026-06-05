using HotelOS.RoomService.Models;
using Xunit;

namespace HotelOS.UnitTests;

public sealed class TS01_RoomFactoryTests
{
    [Fact]
    public void Factory_Creates_SingleRoom_With_Correct_Price()
    {
        var room = RoomFactory.Create(RoomType.Single, "101", 1);

        Assert.Equal(RoomType.Single, room.Type);
        Assert.Equal(120m, room.PricePerNight);
        Assert.Equal(1, room.GuestCapacity);
        Assert.Equal("101", room.RoomNumber);
    }

    [Fact]
    public void Factory_Creates_DoubleRoom_With_Correct_Price()
    {
        var room = RoomFactory.Create(RoomType.Double, "201", 2);

        Assert.Equal(RoomType.Double, room.Type);
        Assert.Equal(220m, room.PricePerNight);
        Assert.Equal(2, room.GuestCapacity);
    }

    [Fact]
    public void Factory_Creates_SuiteRoom_With_Correct_Price()
    {
        var room = RoomFactory.Create(RoomType.Suite, "301", 3);

        Assert.Equal(RoomType.Suite, room.Type);
        Assert.Equal(350m, room.PricePerNight);
        Assert.Equal(4, room.GuestCapacity);
    }

    [Fact]
    public void Factory_Creates_AccessibleRoom_With_Correct_Price()
    {
        var room = RoomFactory.Create(RoomType.Accessible, "102", 1);

        Assert.Equal(RoomType.Accessible, room.Type);
        Assert.Equal(180m, room.PricePerNight);
        Assert.Equal(2, room.GuestCapacity);
    }

    [Fact]
    public void Factory_Throws_For_Unknown_Type()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            RoomFactory.Create((RoomType)999, "X", 1));
    }
}
