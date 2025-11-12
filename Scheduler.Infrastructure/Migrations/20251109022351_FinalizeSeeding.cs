using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Scheduler.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FinalizeSeeding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRemote",
                table: "ScheduleDays");

            migrationBuilder.AddColumn<int>(
                name: "WorkMode",
                table: "ScheduleDays",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WorkMode",
                table: "ScheduleDays");

            migrationBuilder.AddColumn<bool>(
                name: "IsRemote",
                table: "ScheduleDays",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
