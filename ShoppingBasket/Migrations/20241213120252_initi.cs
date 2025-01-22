using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShoppingBasket.Migrations
{
    /// <inheritdoc />
    public partial class initi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ShoppingBaskets",
                columns: table => new
                {
                    ShoppingBasketId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShoppingBaskets", x => x.ShoppingBasketId);
                });

            migrationBuilder.CreateTable(
                name: "BasketLines",
                columns: table => new
                {
                    BasketLineId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShoppingBasketId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    ShoppingBaskettShoppingBasketId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BasketLines", x => x.BasketLineId);
                    table.ForeignKey(
                        name: "FK_BasketLines_ShoppingBaskets_ShoppingBasketId",
                        column: x => x.ShoppingBasketId,
                        principalTable: "ShoppingBaskets",
                        principalColumn: "ShoppingBasketId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BasketLines_ShoppingBaskets_ShoppingBaskettShoppingBasketId",
                        column: x => x.ShoppingBaskettShoppingBasketId,
                        principalTable: "ShoppingBaskets",
                        principalColumn: "ShoppingBasketId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_BasketLines_ShoppingBasketId",
                table: "BasketLines",
                column: "ShoppingBasketId");

            migrationBuilder.CreateIndex(
                name: "IX_BasketLines_ShoppingBaskettShoppingBasketId",
                table: "BasketLines",
                column: "ShoppingBaskettShoppingBasketId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BasketLines");

            migrationBuilder.DropTable(
                name: "ShoppingBaskets");
        }
    }
}
