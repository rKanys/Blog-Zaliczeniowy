using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Blog_Zaliczeniowy.Data.Migrations
{
    /// <inheritdoc />
    public partial class waitingRoomPosts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Approved",
                table: "Posts",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Approved",
                table: "Posts");
        }
    }
}
