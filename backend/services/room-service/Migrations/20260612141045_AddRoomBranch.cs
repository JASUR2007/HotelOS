using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelOS.RoomService.Migrations
{
    /// <inheritdoc />
    public partial class AddRoomBranch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BranchId",
                schema: "room_service",
                table: "rooms",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BranchId",
                schema: "room_service",
                table: "room_keys",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_rooms_BranchId",
                schema: "room_service",
                table: "rooms",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_room_keys_BranchId",
                schema: "room_service",
                table: "room_keys",
                column: "BranchId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_rooms_BranchId",
                schema: "room_service",
                table: "rooms");

            migrationBuilder.DropIndex(
                name: "IX_room_keys_BranchId",
                schema: "room_service",
                table: "room_keys");

            migrationBuilder.DropColumn(
                name: "BranchId",
                schema: "room_service",
                table: "rooms");

            migrationBuilder.DropColumn(
                name: "BranchId",
                schema: "room_service",
                table: "room_keys");
        }
    }
}
