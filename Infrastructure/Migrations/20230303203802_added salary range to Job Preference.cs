using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addedsalaryrangetoJobPreference : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SalaryScale",
                table: "JobPreferences");

            migrationBuilder.AddColumn<decimal>(
                name: "SalaryRangeFrom",
                table: "JobPreferences",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SalaryRangeTo",
                table: "JobPreferences",
                type: "decimal(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SalaryRangeFrom",
                table: "JobPreferences");

            migrationBuilder.DropColumn(
                name: "SalaryRangeTo",
                table: "JobPreferences");

            migrationBuilder.AddColumn<string>(
                name: "SalaryScale",
                table: "JobPreferences",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
