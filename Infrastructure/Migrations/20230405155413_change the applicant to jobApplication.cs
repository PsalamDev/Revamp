using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class changetheapplicanttojobApplication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "ApplicantId",
                table: "ApplicantQuizRecords",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "JobApplicationId",
                table: "ApplicantQuizRecords",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApplicantQuizRecords_JobApplicationId",
                table: "ApplicantQuizRecords",
                column: "JobApplicationId");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicantQuizRecords_JobApplications_JobApplicationId",
                table: "ApplicantQuizRecords",
                column: "JobApplicationId",
                principalTable: "JobApplications",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicantQuizRecords_JobApplications_JobApplicationId",
                table: "ApplicantQuizRecords");

            migrationBuilder.DropIndex(
                name: "IX_ApplicantQuizRecords_JobApplicationId",
                table: "ApplicantQuizRecords");

            migrationBuilder.DropColumn(
                name: "JobApplicationId",
                table: "ApplicantQuizRecords");

            migrationBuilder.AlterColumn<Guid>(
                name: "ApplicantId",
                table: "ApplicantQuizRecords",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);
        }
    }
}
