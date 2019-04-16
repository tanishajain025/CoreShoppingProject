using Microsoft.EntityFrameworkCore.Migrations;

namespace CoreShoppingAdminPortal.Migrations
{
    public partial class PayInitial1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "amount",
                table: "Payments",
                newName: "Amount");

            migrationBuilder.RenameColumn(
                name: "card_no",
                table: "Payments",
                newName: "Cardno");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "Payments",
                newName: "amount");

            migrationBuilder.RenameColumn(
                name: "Cardno",
                table: "Payments",
                newName: "card_no");
        }
    }
}
