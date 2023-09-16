using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedtheJobReviewertable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Comment",
                table: "JobReviewers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                table: "JobReviewers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsReviewd",
                table: "JobReviewers",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "JobReviewers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ApplicantProfileId",
                table: "ApplicantQuizRecords",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApplicantQuizRecords_ApplicantProfileId",
                table: "ApplicantQuizRecords",
                column: "ApplicantProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicantQuizRecords_ApplicantProfiles_ApplicantProfileId",
                table: "ApplicantQuizRecords",
                column: "ApplicantProfileId",
                principalTable: "ApplicantProfiles",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicantQuizRecords_ApplicantProfiles_ApplicantProfileId",
                table: "ApplicantQuizRecords");

            migrationBuilder.DropIndex(
                name: "IX_ApplicantQuizRecords_ApplicantProfileId",
                table: "ApplicantQuizRecords");

            migrationBuilder.DropColumn(
                name: "Comment",
                table: "JobReviewers");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "JobReviewers");

            migrationBuilder.DropColumn(
                name: "IsReviewd",
                table: "JobReviewers");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "JobReviewers");

            migrationBuilder.DropColumn(
                name: "ApplicantProfileId",
                table: "ApplicantQuizRecords");
        }
    }
}
