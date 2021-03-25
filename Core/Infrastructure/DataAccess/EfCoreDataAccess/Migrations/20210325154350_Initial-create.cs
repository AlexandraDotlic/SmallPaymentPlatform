using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Core.Infrastructure.DataAccess.EfCoreDataAccess.Migrations
{
    public partial class Initialcreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Wallets",
                columns: table => new
                {
                    JMBG = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Bank = table.Column<short>(type: "smallint", nullable: false),
                    BankAccountNumber = table.Column<string>(type: "nvarchar(18)", maxLength: 18, nullable: true),
                    BankPIN = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: true),
                    Balance = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    PASS = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wallets", x => x.JMBG);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Amount = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    TransactionDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Type = table.Column<short>(type: "smallint", nullable: false),
                    WalletJMBG = table.Column<string>(type: "nvarchar(13)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transactions_Wallets_WalletJMBG",
                        column: x => x.WalletJMBG,
                        principalTable: "Wallets",
                        principalColumn: "JMBG",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_WalletJMBG",
                table: "Transactions",
                column: "WalletJMBG");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "Wallets");
        }
    }
}
