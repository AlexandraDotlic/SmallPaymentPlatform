using Microsoft.EntityFrameworkCore.Migrations;

namespace Core.Infrastructure.DataAccess.EfCoreDataAccess.Migrations
{
    public partial class Enabled_Cascade_delete_for_Transactions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Wallets_WalletJMBG",
                table: "Transactions");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Wallets_WalletJMBG",
                table: "Transactions",
                column: "WalletJMBG",
                principalTable: "Wallets",
                principalColumn: "JMBG",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Wallets_WalletJMBG",
                table: "Transactions");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Wallets_WalletJMBG",
                table: "Transactions",
                column: "WalletJMBG",
                principalTable: "Wallets",
                principalColumn: "JMBG",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
