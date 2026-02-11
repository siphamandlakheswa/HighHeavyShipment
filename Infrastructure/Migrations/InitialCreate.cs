using Microsoft.EntityFrameworkCore.Migrations;

namespace HighHeavyShipment.Infrastructure.Migrations;

public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Shipments",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Reference = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                Mode = table.Column<int>(type: "int", nullable: false),
                Origin = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                Destination = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                WeightKg = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                Status = table.Column<int>(type: "int", nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Shipments", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "ShipmentQuotes",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ShipmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Phase = table.Column<int>(type: "int", nullable: false),
                Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ShipmentQuotes", x => x.Id);
                table.ForeignKey(
                    name: "FK_ShipmentQuotes_Shipments_ShipmentId",
                    column: x => x.ShipmentId,
                    principalTable: "Shipments",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Shipments_Reference",
            table: "Shipments",
            column: "Reference",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_ShipmentQuotes_ShipmentId_Phase",
            table: "ShipmentQuotes",
            columns: new[] { "ShipmentId", "Phase" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_ShipmentQuotes_ShipmentId",
            table: "ShipmentQuotes",
            column: "ShipmentId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "ShipmentQuotes");
        migrationBuilder.DropTable(name: "Shipments");
    }
}
