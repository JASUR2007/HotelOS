using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

using Microsoft.EntityFrameworkCore.Infrastructure;
using HotelOS.RoomService.Data;

namespace HotelOS.RoomService.Migrations
{
    [DbContext(typeof(RoomDbContext))]
    [Migration("20260601120000_AddRoomsAndAmenities")]
    public partial class AddRoomsAndAmenities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "amenities",
                schema: "room_service",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IconUrl = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_amenities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "rooms",
                schema: "room_service",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoomNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PricePerNight = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    Floor = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    GuestCapacity = table.Column<int>(type: "integer", nullable: false),
                    MainImage = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Images = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rooms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "room_amenities",
                schema: "room_service",
                columns: table => new
                {
                    amenity_id = table.Column<int>(type: "integer", nullable: false),
                    room_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_room_amenities", x => new { x.amenity_id, x.room_id });
                    table.ForeignKey(
                        name: "FK_room_amenities_amenities_amenity_id",
                        column: x => x.amenity_id,
                        principalSchema: "room_service",
                        principalTable: "amenities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_room_amenities_rooms_room_id",
                        column: x => x.room_id,
                        principalSchema: "room_service",
                        principalTable: "rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "room_service",
                table: "amenities",
                columns: new[] { "Id", "Name", "IconUrl", "Description" },
                columnTypes: new[] { "integer", "character varying(100)", "character varying(300)", "character varying(500)" },
                values: new object[,]
                {
                    { 1, "WiFi", "/images/amenities/wifi.svg", "High-speed internet access" },
                    { 2, "Coffee Machine", "/images/amenities/coffee.svg", "Coffee machine with complimentary pods" },
                    { 3, "Air Conditioning", "/images/amenities/ac.svg", "Climate control system" },
                    { 4, "Smart TV", "/images/amenities/tv.svg", "Smart TV with streaming services" },
                    { 5, "TV", "/images/amenities/tv.svg", "Flat-screen television" },
                    { 6, "Mini Bar", "/images/amenities/minibar.svg", "Stocked mini bar" },
                    { 7, "Bath", "/images/amenities/bath.svg", "Private bathroom" },
                    { 8, "Jacuzzi", "/images/amenities/jacuzzi.svg", "Whirlpool bathtub" },
                    { 9, "Balcony", "/images/amenities/balcony.svg", "Private balcony with view" },
                    { 10, "Wheelchair Access", "/images/amenities/wheelchair.svg", "Wheelchair accessible" },
                    { 11, "Roll-in Shower", "/images/amenities/shower.svg", "Accessible roll-in shower" },
                    { 12, "Butler Service", "/images/amenities/butler.svg", "Dedicated butler service" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_amenities_Name",
                schema: "room_service",
                table: "amenities",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_room_amenities_room_id",
                schema: "room_service",
                table: "room_amenities",
                column: "room_id");

            migrationBuilder.CreateIndex(
                name: "IX_rooms_RoomNumber",
                schema: "room_service",
                table: "rooms",
                column: "RoomNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_rooms_Status",
                schema: "room_service",
                table: "rooms",
                column: "Status");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "room_amenities",
                schema: "room_service");

            migrationBuilder.DropTable(
                name: "amenities",
                schema: "room_service");

            migrationBuilder.DropTable(
                name: "rooms",
                schema: "room_service");
        }
    }
}
