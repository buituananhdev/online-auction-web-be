using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineAuctionWeb.Domain.Migrations
{
    /// <inheritdoc />
    public partial class ChangeSellerToUserInAuction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Auctions_Users_SellerId",
                table: "Auctions");

            migrationBuilder.RenameColumn(
                name: "SellerId",
                table: "Auctions",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Auctions_SellerId",
                table: "Auctions",
                newName: "IX_Auctions_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Auctions_Users_UserId",
                table: "Auctions",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Auctions_Users_UserId",
                table: "Auctions");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Auctions",
                newName: "SellerId");

            migrationBuilder.RenameIndex(
                name: "IX_Auctions_UserId",
                table: "Auctions",
                newName: "IX_Auctions_SellerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Auctions_Users_SellerId",
                table: "Auctions",
                column: "SellerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
