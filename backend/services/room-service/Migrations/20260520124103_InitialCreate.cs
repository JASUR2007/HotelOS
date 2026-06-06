using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HotelOS.RoomService.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "room_service");

            migrationBuilder.CreateTable(
                name: "menu_items",
                schema: "room_service",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Price = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_menu_items", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "orders",
                schema: "room_service",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoomNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    GuestName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Total = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orders", x => x.Id);
                });

            migrationBuilder.InsertData(
                schema: "room_service",
                table: "menu_items",
                columns: new[] { "Id", "Name", "Price" },
                values: new object[,]
                {
                    { 1, "Breakfast set", 18m },
                    { 2, "Club sandwich", 14m },
                    { 3, "Sparkling water", 4m }
                });

            migrationBuilder.InsertData(
                schema: "room_service",
                table: "orders",
                columns: new[] { "Id", "GuestName", "RoomNumber", "Status", "Total" },
                values: new object[,]
                {
                    { 1, "Amelia Stone", "101", "Preparing", 24m },
                    { 2, "Daniel Reed", "207", "Out for delivery", 18m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_orders_RoomNumber_Status",
                schema: "room_service",
                table: "orders",
                columns: new[] { "RoomNumber", "Status" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "menu_items",
                schema: "room_service");

            migrationBuilder.DropTable(
                name: "orders",
                schema: "room_service");
        }
    }
}