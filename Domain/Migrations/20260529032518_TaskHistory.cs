using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Domain.Migrations
{
    /// <inheritdoc />
    public partial class TaskHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TaskHistory",
                schema: "Tasks",
                columns: table => new
                {
                    IdTaskHistory = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdTaskItem = table.Column<long>(type: "bigint", nullable: false),
                    IdUser = table.Column<long>(type: "bigint", nullable: false),
                    PreviousStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NewStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ChangedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DurationMinutes = table.Column<int>(type: "int", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    UsuarioRegistro = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IpRegistro = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UsuarioModificacion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", maxLength: 100, nullable: true),
                    IpModificacion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UsuarioEliminacion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaEliminacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IpEliminacion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskHistory", x => x.IdTaskHistory);
                    table.ForeignKey(
                        name: "FK_TaskHistory_TaskItem_IdTaskItem",
                        column: x => x.IdTaskItem,
                        principalSchema: "Tasks",
                        principalTable: "TaskItem",
                        principalColumn: "IdTaskItem",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaskHistory_User_IdUser",
                        column: x => x.IdUser,
                        principalSchema: "SEG",
                        principalTable: "User",
                        principalColumn: "IdUser");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TaskHistory_IdTaskItem",
                schema: "Tasks",
                table: "TaskHistory",
                column: "IdTaskItem");

            migrationBuilder.CreateIndex(
                name: "IX_TaskHistory_IdUser",
                schema: "Tasks",
                table: "TaskHistory",
                column: "IdUser");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TaskHistory",
                schema: "Tasks");
        }
    }
}
