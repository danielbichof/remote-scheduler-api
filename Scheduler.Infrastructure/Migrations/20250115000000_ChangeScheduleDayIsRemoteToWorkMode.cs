using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Scheduler.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeScheduleDayIsRemoteToWorkMode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Adiciona a nova coluna WorkMode (nullable int)
            migrationBuilder.AddColumn<int>(
                name: "WorkMode",
                table: "ScheduleDays",
                type: "integer",
                nullable: true);

            // Migra os dados: IsRemote true -> WorkMode 0 (Remote), IsRemote false -> WorkMode 1 (Office)
            migrationBuilder.Sql(@"
                UPDATE ""ScheduleDays""
                SET ""WorkMode"" = CASE 
                    WHEN ""IsRemote"" = true THEN 0
                    WHEN ""IsRemote"" = false THEN 1
                    ELSE NULL
                END
            ");

            // Remove a coluna antiga IsRemote
            migrationBuilder.DropColumn(
                name: "IsRemote",
                table: "ScheduleDays");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Adiciona a coluna IsRemote de volta
            migrationBuilder.AddColumn<bool>(
                name: "IsRemote",
                table: "ScheduleDays",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            // Migra os dados de volta: WorkMode 0 -> IsRemote true, outros -> IsRemote false
            migrationBuilder.Sql(@"
                UPDATE ""ScheduleDays""
                SET ""IsRemote"" = CASE 
                    WHEN ""WorkMode"" = 0 THEN true
                    ELSE false
                END
            ");

            // Remove a coluna WorkMode
            migrationBuilder.DropColumn(
                name: "WorkMode",
                table: "ScheduleDays");
        }
    }
}

