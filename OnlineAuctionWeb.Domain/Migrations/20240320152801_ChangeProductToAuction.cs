using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineAuctionWeb.Domain.Migrations
{
    /// <inheritdoc />
    public partial class ChangeProductToAuction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bids_Products_ProductId",
                table: "Bids");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductMedias_Products_ProductId",
                table: "ProductMedias");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Categories_CategoryId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Users_SellerId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_ProductMedias_ProductId",
                table: "ProductMedias");

            migrationBuilder.DropIndex(
                name: "IX_Bids_ProductId",
                table: "Bids");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Products",
                table: "Products");

            migrationBuilder.RenameTable(
                name: "Products",
                newName: "Auctions");

            migrationBuilder.RenameIndex(
                name: "IX_Products_SellerId",
                table: "Auctions",
                newName: "IX_Auctions_SellerId");

            migrationBuilder.RenameIndex(
                name: "IX_Products_CategoryId",
                table: "Auctions",
                newName: "IX_Auctions_CategoryId");

            migrationBuilder.AddColumn<int>(
                name: "AuctionId",
                table: "ProductMedias",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AuctionId",
                table: "Bids",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "MaxPrice",
                table: "Auctions",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Auctions",
                table: "Auctions",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ProductMedias_AuctionId",
                table: "ProductMedias",
                column: "AuctionId");

            migrationBuilder.CreateIndex(
                name: "IX_Bids_AuctionId",
                table: "Bids",
                column: "AuctionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Auctions_Categories_CategoryId",
                table: "Auctions",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Auctions_Users_SellerId",
                table: "Auctions",
                column: "SellerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Bids_Auctions_AuctionId",
                table: "Bids",
                column: "AuctionId",
                principalTable: "Auctions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductMedias_Auctions_AuctionId",
                table: "ProductMedias",
                column: "AuctionId",
                principalTable: "Auctions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Auctions_Categories_CategoryId",
                table: "Auctions");

            migrationBuilder.DropForeignKey(
                name: "FK_Auctions_Users_SellerId",
                table: "Auctions");

            migrationBuilder.DropForeignKey(
                name: "FK_Bids_Auctions_AuctionId",
                table: "Bids");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductMedias_Auctions_AuctionId",
                table: "ProductMedias");

            migrationBuilder.DropIndex(
                name: "IX_ProductMedias_AuctionId",
                table: "ProductMedias");

            migrationBuilder.DropIndex(
                name: "IX_Bids_AuctionId",
                table: "Bids");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Auctions",
                table: "Auctions");

            migrationBuilder.DropColumn(
                name: "AuctionId",
                table: "ProductMedias");

            migrationBuilder.DropColumn(
                name: "AuctionId",
                table: "Bids");

            migrationBuilder.DropColumn(
                name: "MaxPrice",
                table: "Auctions");

            migrationBuilder.RenameTable(
                name: "Auctions",
                newName: "Products");

            migrationBuilder.RenameIndex(
                name: "IX_Auctions_SellerId",
                table: "Products",
                newName: "IX_Products_SellerId");

            migrationBuilder.RenameIndex(
                name: "IX_Auctions_CategoryId",
                table: "Products",
                newName: "IX_Products_CategoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Products",
                table: "Products",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ProductMedias_ProductId",
                table: "ProductMedias",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Bids_ProductId",
                table: "Bids",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bids_Products_ProductId",
                table: "Bids",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductMedias_Products_ProductId",
                table: "ProductMedias",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Categories_CategoryId",
                table: "Products",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Users_SellerId",
                table: "Products",
                column: "SellerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
