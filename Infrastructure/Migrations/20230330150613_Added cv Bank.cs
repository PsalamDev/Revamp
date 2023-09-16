using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedcvBank : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobApplicantCVBanks_JobApplications_JobApplicationId",
                table: "JobApplicantCVBanks");

            migrationBuilder.AlterColumn<Guid>(
                name: "JobApplicationId",
                table: "JobApplicantCVBanks",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "FileType",
                table: "JobApplicantCVBanks",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "FileName",
                table: "JobApplicantCVBanks",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<Guid>(
                name: "ApplicantionId",
                table: "JobApplicantCVBanks",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "ApplicantId",
                table: "JobApplicantCVBanks",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ApplicantProfileId",
                table: "JobApplicantCVBanks",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_JobApplicantCVBanks_ApplicantProfileId",
                table: "JobApplicantCVBanks",
                column: "ApplicantProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_JobApplicantCVBanks_ApplicantProfiles_ApplicantProfileId",
                table: "JobApplicantCVBanks",
                column: "ApplicantProfileId",
                principalTable: "ApplicantProfiles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_JobApplicantCVBanks_JobApplications_JobApplicationId",
                table: "JobApplicantCVBanks",
                column: "JobApplicationId",
                principalTable: "JobApplications",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobApplicantCVBanks_ApplicantProfiles_ApplicantProfileId",
                table: "JobApplicantCVBanks");

            migrationBuilder.DropForeignKey(
                name: "FK_JobApplicantCVBanks_JobApplications_JobApplicationId",
                table: "JobApplicantCVBanks");

            migrationBuilder.DropIndex(
                name: "IX_JobApplicantCVBanks_ApplicantProfileId",
                table: "JobApplicantCVBanks");

            migrationBuilder.DropColumn(
                name: "ApplicantId",
                table: "JobApplicantCVBanks");

            migrationBuilder.DropColumn(
                name: "ApplicantProfileId",
                table: "JobApplicantCVBanks");

            migrationBuilder.AlterColumn<Guid>(
                name: "JobApplicationId",
                table: "JobApplicantCVBanks",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FileType",
                table: "JobApplicantCVBanks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FileName",
                table: "JobApplicantCVBanks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ApplicantionId",
                table: "JobApplicantCVBanks",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_JobApplicantCVBanks_JobApplications_JobApplicationId",
                table: "JobApplicantCVBanks",
                column: "JobApplicationId",
                principalTable: "JobApplications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
