using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineAuctionWeb.Domain.Migrations
{
    /// <inheritdoc />
    public partial class PredictAvgPriceInAuctionTbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PredictAvgPrice",
                table: "Auctions",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PredictAvgPrice",
                table: "Auctions");
        }
    }
}
