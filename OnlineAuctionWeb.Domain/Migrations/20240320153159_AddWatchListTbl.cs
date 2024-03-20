using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineAuctionWeb.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddWatchListTbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductMedias_Auctions_AuctionId",
                table: "ProductMedias");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductMedias",
                table: "ProductMedias");

            migrationBuilder.RenameTable(
                name: "ProductMedias",
                newName: "AuctionMedias");

            migrationBuilder.RenameIndex(
                name: "IX_ProductMedias_AuctionId",
                table: "AuctionMedias",
                newName: "IX_AuctionMedias_AuctionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AuctionMedias",
                table: "AuctionMedias",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "WatchList",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    AuctionId = table.Column<int>(type: "int", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WatchList", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WatchList_Auctions_AuctionId",
                        column: x => x.AuctionId,
                        principalTable: "Auctions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WatchList_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WatchList_AuctionId",
                table: "WatchList",
                column: "AuctionId");

            migrationBuilder.CreateIndex(
                name: "IX_WatchList_UserId",
                table: "WatchList",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AuctionMedias_Auctions_AuctionId",
                table: "AuctionMedias",
                column: "AuctionId",
                principalTable: "Auctions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuctionMedias_Auctions_AuctionId",
                table: "AuctionMedias");

            migrationBuilder.DropTable(
                name: "WatchList");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AuctionMedias",
                table: "AuctionMedias");

            migrationBuilder.RenameTable(
                name: "AuctionMedias",
                newName: "ProductMedias");

            migrationBuilder.RenameIndex(
                name: "IX_AuctionMedias_AuctionId",
                table: "ProductMedias",
                newName: "IX_ProductMedias_AuctionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductMedias",
                table: "ProductMedias",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductMedias_Auctions_AuctionId",
                table: "ProductMedias",
                column: "AuctionId",
                principalTable: "Auctions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
