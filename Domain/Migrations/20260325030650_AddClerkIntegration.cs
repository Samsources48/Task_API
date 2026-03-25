using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddClerkIntegration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordHash",
                schema: "SEG",
                table: "User");

            migrationBuilder.AddColumn<string>(
                name: "ClerkId",
                schema: "SEG",
                table: "User",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                schema: "SEG",
                table: "User",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_User_ClerkId",
                schema: "SEG",
                table: "User",
                column: "ClerkId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_User_ClerkId",
                schema: "SEG",
                table: "User");

            migrationBuilder.DropColumn(
                name: "ClerkId",
                schema: "SEG",
                table: "User");

            migrationBuilder.DropColumn(
                name: "Email",
                schema: "SEG",
                table: "User");

            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                schema: "SEG",
                table: "User",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
