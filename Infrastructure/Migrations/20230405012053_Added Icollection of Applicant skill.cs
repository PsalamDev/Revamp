using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedIcollectionofApplicantskill : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ApplicantProfileId",
                table: "ApplicantSkills",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApplicantSkills_ApplicantProfileId",
                table: "ApplicantSkills",
                column: "ApplicantProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicantSkills_ApplicantProfiles_ApplicantProfileId",
                table: "ApplicantSkills",
                column: "ApplicantProfileId",
                principalTable: "ApplicantProfiles",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicantSkills_ApplicantProfiles_ApplicantProfileId",
                table: "ApplicantSkills");

            migrationBuilder.DropIndex(
                name: "IX_ApplicantSkills_ApplicantProfileId",
                table: "ApplicantSkills");

            migrationBuilder.DropColumn(
                name: "ApplicantProfileId",
                table: "ApplicantSkills");
        }
    }
}
