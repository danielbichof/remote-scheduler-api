using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Scheduler.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Groups",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Grupo inicial para novos usu�rios", "Grupo Padr�o" });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "DisplayName",
                value: "Usu�rio");

            migrationBuilder.UpdateData(
                table: "Schedules",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "Title" },
                values: new object[] { "Hor�rio de fallback inicial.", "Hor�rio Padr�o" });

            migrationBuilder.UpdateData(
                table: "Weekdays",
                keyColumn: "Id",
                keyValue: 3,
                column: "DayName",
                value: "Ter�a-feira");

            migrationBuilder.UpdateData(
                table: "Weekdays",
                keyColumn: "Id",
                keyValue: 7,
                column: "DayName",
                value: "S�bado");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Groups",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Grupo inicial para novos usuários", "Grupo Padrão" });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "DisplayName",
                value: "Usuário");

            migrationBuilder.UpdateData(
                table: "Schedules",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "Title" },
                values: new object[] { "Horário de fallback inicial.", "Horário Padrão" });

            migrationBuilder.UpdateData(
                table: "Weekdays",
                keyColumn: "Id",
                keyValue: 3,
                column: "DayName",
                value: "Terça-feira");

            migrationBuilder.UpdateData(
                table: "Weekdays",
                keyColumn: "Id",
                keyValue: 7,
                column: "DayName",
                value: "Sábado");
        }
    }
}
