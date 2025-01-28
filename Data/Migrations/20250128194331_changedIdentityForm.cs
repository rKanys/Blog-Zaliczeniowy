using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Blog_Zaliczeniowy.Data.Migrations
{
    /// <inheritdoc />
    public partial class changedIdentityForm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FullName",
                table: "AspNetUsers",
                newName: "Nickname");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Nickname",
                table: "AspNetUsers",
                newName: "FullName");
        }
    }
}
