using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Core.Infrastructure.DataAccess.EfCoreDataAccess.Migrations
{
    public partial class Updated_tables_Wallet_and_Transaction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsBlocked",
                table: "Wallets",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastTransactionDateTime",
                table: "Wallets",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<decimal>(
                name: "UsedDepositForCurrentMonth",
                table: "Wallets",
                type: "decimal(12,2)",
                precision: 12,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "UsedWithdrawalForCurrentMonth",
                table: "Wallets",
                type: "decimal(12,2)",
                precision: 12,
                scale: 2,
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsBlocked",
                table: "Wallets");

            migrationBuilder.DropColumn(
                name: "LastTransactionDateTime",
                table: "Wallets");

            migrationBuilder.DropColumn(
                name: "UsedDepositForCurrentMonth",
                table: "Wallets");

            migrationBuilder.DropColumn(
                name: "UsedWithdrawalForCurrentMonth",
                table: "Wallets");
        }
    }
}
