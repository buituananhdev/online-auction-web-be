using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineAuctionWeb.Domain.Migrations
{
    /// <inheritdoc />
    public partial class ChangeIsUniqueInFeedbackIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Feedbacks_FromUserId_ToUserId",
                table: "Feedbacks");

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_FromUserId_ToUserId",
                table: "Feedbacks",
                columns: new[] { "FromUserId", "ToUserId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Feedbacks_FromUserId_ToUserId",
                table: "Feedbacks");

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_FromUserId_ToUserId",
                table: "Feedbacks",
                columns: new[] { "FromUserId", "ToUserId" },
                unique: true);
        }
    }
}
