using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Blog_Zaliczeniowy.Data.Migrations
{
    /// <inheritdoc />
    public partial class PostAndCommentVisibility : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Visibility",
                table: "Posts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Visibility",
                table: "Comments",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Visibility",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "Visibility",
                table: "Comments");
        }
    }
}
