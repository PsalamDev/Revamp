using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addedstagetotheJobApplication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "StageId",
                table: "JobApplications",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_JobApplications_StageId",
                table: "JobApplications",
                column: "StageId");

            migrationBuilder.AddForeignKey(
                name: "FK_JobApplications_Stages_StageId",
                table: "JobApplications",
                column: "StageId",
                principalTable: "Stages",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobApplications_Stages_StageId",
                table: "JobApplications");

            migrationBuilder.DropIndex(
                name: "IX_JobApplications_StageId",
                table: "JobApplications");

            migrationBuilder.DropColumn(
                name: "StageId",
                table: "JobApplications");
        }
    }
}
