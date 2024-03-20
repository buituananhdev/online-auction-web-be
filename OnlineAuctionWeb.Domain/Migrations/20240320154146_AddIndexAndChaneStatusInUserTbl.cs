using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineAuctionWeb.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexAndChaneStatusInUserTbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Feedbacks_FromUserId",
                table: "Feedbacks");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "Users",
                newName: "Status");

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "Users",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "CategoryName",
                table: "Categories",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ProductName",
                table: "Auctions",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email_FullName_Status_DateCreated",
                table: "Users",
                columns: new[] { "Email", "FullName", "Status", "DateCreated" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_FromUserId_ToUserId",
                table: "Feedbacks",
                columns: new[] { "FromUserId", "ToUserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_CategoryName",
                table: "Categories",
                column: "CategoryName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Auctions_ProductName_ProductStatus_DateCreated",
                table: "Auctions",
                columns: new[] { "ProductName", "ProductStatus", "DateCreated" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_Email_FullName_Status_DateCreated",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Feedbacks_FromUserId_ToUserId",
                table: "Feedbacks");

            migrationBuilder.DropIndex(
                name: "IX_Categories_CategoryName",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Auctions_ProductName_ProductStatus_DateCreated",
                table: "Auctions");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Users",
                newName: "IsActive");

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "CategoryName",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "ProductName",
                table: "Auctions",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_FromUserId",
                table: "Feedbacks",
                column: "FromUserId");
        }
    }
}
