using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CASU.Migrations
{
    /// <inheritdoc />
    public partial class AddProductionLogsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductionHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CurrentDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CurrentTime = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmpCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmpName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MachineCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaLenh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Specification = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SanLuongDat = table.Column<int>(type: "int", nullable: false),
                    ChiTieu = table.Column<int>(type: "int", nullable: false),
                    QrCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Position = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItemCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QuyCach = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaGai = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NhanHieu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SoMaHoa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaDot = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductionHistories", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductionHistories");
        }
    }
}
