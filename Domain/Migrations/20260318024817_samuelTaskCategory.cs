using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Domain.Migrations
{
    /// <inheritdoc />
    public partial class samuelTaskCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DueData",
                schema: "Tasks",
                table: "TaskItem",
                newName: "StartDate");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                schema: "Tasks",
                table: "TaskItem",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "IdTaskCategory",
                schema: "Tasks",
                table: "TaskItem",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TaskCategory",
                schema: "Tasks",
                columns: table => new
                {
                    IdTaskCategory = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    UsuarioRegistro = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IpRegistro = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UsuarioModificacion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", maxLength: 100, nullable: true),
                    IpModificacion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UsuarioEliminacion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaEliminacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IpEliminacion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskCategory", x => x.IdTaskCategory);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TaskItem_IdTaskCategory",
                schema: "Tasks",
                table: "TaskItem",
                column: "IdTaskCategory");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskItem_TaskCategory_IdTaskCategory",
                schema: "Tasks",
                table: "TaskItem",
                column: "IdTaskCategory",
                principalSchema: "Tasks",
                principalTable: "TaskCategory",
                principalColumn: "IdTaskCategory",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskItem_TaskCategory_IdTaskCategory",
                schema: "Tasks",
                table: "TaskItem");

            migrationBuilder.DropTable(
                name: "TaskCategory",
                schema: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_TaskItem_IdTaskCategory",
                schema: "Tasks",
                table: "TaskItem");

            migrationBuilder.DropColumn(
                name: "EndDate",
                schema: "Tasks",
                table: "TaskItem");

            migrationBuilder.DropColumn(
                name: "IdTaskCategory",
                schema: "Tasks",
                table: "TaskItem");

            migrationBuilder.RenameColumn(
                name: "StartDate",
                schema: "Tasks",
                table: "TaskItem",
                newName: "DueData");
        }
    }
}
