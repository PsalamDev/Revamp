using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class changethequiz : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ApplicantQuizRecords_QuizId",
                table: "ApplicantQuizRecords",
                column: "QuizId");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicantQuizRecords_Quizzes_QuizId",
                table: "ApplicantQuizRecords",
                column: "QuizId",
                principalTable: "Quizzes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicantQuizRecords_Quizzes_QuizId",
                table: "ApplicantQuizRecords");

            migrationBuilder.DropIndex(
                name: "IX_ApplicantQuizRecords_QuizId",
                table: "ApplicantQuizRecords");
        }
    }
}
