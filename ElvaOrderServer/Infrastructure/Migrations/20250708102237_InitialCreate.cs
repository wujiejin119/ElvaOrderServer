using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ElvaOrderServer.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Orders",
                columns: new[] { "Id", "CreatedAt", "CustomerName" },
                values: new object[,]
                {
                    { new Guid("a8b3c7d0-9e1f-4a6b-8c3d-2e5f4a6b8c9d"), new DateTime(2023, 6, 15, 10, 30, 0, 0, DateTimeKind.Utc), "John Smith" },
                    { new Guid("b9c4d8e1-0f2a-5b7c-9d3e-1f6a7b8c9d0e"), new DateTime(2023, 6, 16, 14, 45, 0, 0, DateTimeKind.Utc), "Emma Johnson" }
                });

            migrationBuilder.InsertData(
                table: "OrderItems",
                columns: new[] { "Id", "OrderId", "ProductId", "Quantity" },
                values: new object[,]
                {
                    { 1, new Guid("a8b3c7d0-9e1f-4a6b-8c3d-2e5f4a6b8c9d"), new Guid("f47ac10b-58cc-4372-a567-0e02b2c3d479"), 2 },
                    { 2, new Guid("a8b3c7d0-9e1f-4a6b-8c3d-2e5f4a6b8c9d"), new Guid("550e8400-e29b-41d4-a716-446655440000"), 1 },
                    { 3, new Guid("b9c4d8e1-0f2a-5b7c-9d3e-1f6a7b8c9d0e"), new Guid("550e8400-e29b-41d4-a716-446655440000"), 3 },
                    { 4, new Guid("b9c4d8e1-0f2a-5b7c-9d3e-1f6a7b8c9d0e"), new Guid("6ba7b810-9dad-11d1-80b4-00c04fd430c8"), 1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItems",
                column: "OrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "Orders");
        }
    }
}
