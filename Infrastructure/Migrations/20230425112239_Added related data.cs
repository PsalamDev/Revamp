using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Addedrelateddata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_JobScheduleInterviews_HireStageId",
                table: "JobScheduleInterviews",
                column: "HireStageId");

            migrationBuilder.CreateIndex(
                name: "IX_JobScheduleInterviews_SubhireStageId",
                table: "JobScheduleInterviews",
                column: "SubhireStageId");

            migrationBuilder.AddForeignKey(
                name: "FK_JobScheduleInterviews_Stages_HireStageId",
                table: "JobScheduleInterviews",
                column: "HireStageId",
                principalTable: "Stages",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_JobScheduleInterviews_SubStages_SubhireStageId",
                table: "JobScheduleInterviews",
                column: "SubhireStageId",
                principalTable: "SubStages",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobScheduleInterviews_Stages_HireStageId",
                table: "JobScheduleInterviews");

            migrationBuilder.DropForeignKey(
                name: "FK_JobScheduleInterviews_SubStages_SubhireStageId",
                table: "JobScheduleInterviews");

            migrationBuilder.DropIndex(
                name: "IX_JobScheduleInterviews_HireStageId",
                table: "JobScheduleInterviews");

            migrationBuilder.DropIndex(
                name: "IX_JobScheduleInterviews_SubhireStageId",
                table: "JobScheduleInterviews");
        }
    }
}
