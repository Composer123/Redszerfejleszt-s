using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Raktar.DataContext.Migrations
{
    /// <inheritdoc />
    public partial class Initialization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    AddressId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Addresse__091C2AFBE08D2200", x => x.AddressId);
                });

            migrationBuilder.CreateTable(
                name: "Feedback",
                columns: table => new
                {
                    FeedbackID = table.Column<int>(type: "int", nullable: false),
                    FeedbackText = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Feedback__6A4BEDF6CB67F50F", x => x.FeedbackID);
                });

            migrationBuilder.CreateTable(
                name: "Product",
                columns: table => new
                {
                    ProductID = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(350)", maxLength: 350, nullable: true),
                    Price = table.Column<decimal>(type: "money", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    Stock = table.Column<int>(type: "int", nullable: true),
                    MaxQuantityPerBlock = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Product__B40CC6ED8D232CD2", x => x.ProductID);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    RoleID = table.Column<int>(type: "int", nullable: false),
                    RoleName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Role__8AFACE3A3E44BD2B", x => x.RoleID);
                });

            migrationBuilder.CreateTable(
                name: "Settlements",
                columns: table => new
                {
                    SettlementId = table.Column<int>(type: "int", nullable: false),
                    PostCode = table.Column<int>(type: "int", nullable: true),
                    SettlementName = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Settleme__7712545A878A8AE7", x => x.SettlementId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    TelephoneNumber = table.Column<int>(type: "int", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: true),
                    Username = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: true),
                    Password = table.Column<byte[]>(type: "varbinary(128)", maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Users__1788CC4C468AD53B", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Blocks",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "int", nullable: true),
                    StorageId = table.Column<int>(type: "int", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.ForeignKey(
                        name: "FK__Blocks__ProductI__403A8C7D",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "ProductID");
                });

            migrationBuilder.CreateTable(
                name: "LandRegistryNumbers",
                columns: table => new
                {
                    AddressId = table.Column<int>(type: "int", nullable: false),
                    SettlementId = table.Column<int>(type: "int", nullable: false),
                    Contents = table.Column<string>(type: "varchar(60)", unicode: false, maxLength: 60, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__LandRegi__091C2AFB62847A28", x => x.AddressId);
                    table.ForeignKey(
                        name: "FK__LandRegis__Addre__4D94879B",
                        column: x => x.AddressId,
                        principalTable: "Addresses",
                        principalColumn: "AddressId");
                    table.ForeignKey(
                        name: "FK__LandRegis__Settl__4E88ABD4",
                        column: x => x.SettlementId,
                        principalTable: "Settlements",
                        principalColumn: "SettlementId");
                });

            migrationBuilder.CreateTable(
                name: "SimpleAddresses",
                columns: table => new
                {
                    AddressId = table.Column<int>(type: "int", nullable: false),
                    SettlementId = table.Column<int>(type: "int", nullable: false),
                    StreetName = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    StreetType = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    HouseNumber = table.Column<int>(type: "int", nullable: true),
                    StairwayNumber = table.Column<int>(type: "int", nullable: true),
                    FloorNumber = table.Column<int>(type: "int", nullable: true),
                    DoorNumber = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__SimpleAd__091C2AFB241679E9", x => x.AddressId);
                    table.ForeignKey(
                        name: "FK__SimpleAdd__Addre__49C3F6B7",
                        column: x => x.AddressId,
                        principalTable: "Addresses",
                        principalColumn: "AddressId");
                    table.ForeignKey(
                        name: "FK__SimpleAdd__Settl__4AB81AF0",
                        column: x => x.SettlementId,
                        principalTable: "Settlements",
                        principalColumn: "SettlementId");
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    OrderID = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    DeliveryAdressID = table.Column<int>(type: "int", nullable: true),
                    OrderDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    DeliveryDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    Status = table.Column<string>(type: "varchar(1)", unicode: false, maxLength: 1, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Orders__C3905BAFC81DC18F", x => x.OrderID);
                    table.ForeignKey(
                        name: "FK__Orders__Delivery__52593CB8",
                        column: x => x.DeliveryAdressID,
                        principalTable: "Addresses",
                        principalColumn: "AddressId");
                    table.ForeignKey(
                        name: "FK__Orders__UserId__5165187F",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "Privilages",
                columns: table => new
                {
                    RoleID = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Privilag__5B8242FE629EC484", x => new { x.RoleID, x.UserId });
                    table.ForeignKey(
                        name: "FK__Privilage__RoleI__3B75D760",
                        column: x => x.RoleID,
                        principalTable: "Role",
                        principalColumn: "RoleID");
                    table.ForeignKey(
                        name: "FK__Privilage__UserI__3C69FB99",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "Cart",
                columns: table => new
                {
                    ProductID = table.Column<int>(type: "int", nullable: false),
                    OrderID = table.Column<int>(type: "int", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: true),
                    FeedbackId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Cart__B40CC6ED54605DBE", x => x.ProductID);
                    table.ForeignKey(
                        name: "FK__Cart__FeedbackId__5629CD9C",
                        column: x => x.FeedbackId,
                        principalTable: "Feedback",
                        principalColumn: "FeedbackID");
                    table.ForeignKey(
                        name: "FK__Cart__OrderID__5535A963",
                        column: x => x.OrderID,
                        principalTable: "Orders",
                        principalColumn: "OrderID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Blocks_ProductId",
                table: "Blocks",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Cart_FeedbackId",
                table: "Cart",
                column: "FeedbackId");

            migrationBuilder.CreateIndex(
                name: "IX_Cart_OrderID",
                table: "Cart",
                column: "OrderID");

            migrationBuilder.CreateIndex(
                name: "IX_LandRegistryNumbers_SettlementId",
                table: "LandRegistryNumbers",
                column: "SettlementId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_DeliveryAdressID",
                table: "Orders",
                column: "DeliveryAdressID");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UserId",
                table: "Orders",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Privilages_UserId",
                table: "Privilages",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "idx_PostCode",
                table: "Settlements",
                column: "PostCode");

            migrationBuilder.CreateIndex(
                name: "IX_SimpleAddresses_SettlementId",
                table: "SimpleAddresses",
                column: "SettlementId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Blocks");

            migrationBuilder.DropTable(
                name: "Cart");

            migrationBuilder.DropTable(
                name: "LandRegistryNumbers");

            migrationBuilder.DropTable(
                name: "Privilages");

            migrationBuilder.DropTable(
                name: "SimpleAddresses");

            migrationBuilder.DropTable(
                name: "Product");

            migrationBuilder.DropTable(
                name: "Feedback");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropTable(
                name: "Settlements");

            migrationBuilder.DropTable(
                name: "Addresses");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
