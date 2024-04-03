using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Works",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SenderWorkCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    RecordCode = table.Column<string>(type: "nvarchar(1)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Role = table.Column<string>(type: "nvarchar(1)", nullable: true),
                    ShareHolder = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IPI = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    InWorkPR = table.Column<int>(type: "int", nullable: true),
                    InWorkMR = table.Column<int>(type: "int", nullable: true),
                    Controlled = table.Column<string>(type: "nvarchar(1)", nullable: true),
                    ISWC = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AgreementNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Works", x => x.ID);
                });

            migrationBuilder.InsertData(
                table: "Works",
                columns: new[] { "ID", "AgreementNumber", "Controlled", "IPI", "ISWC", "InWorkMR", "InWorkPR", "RecordCode", "Role", "SenderWorkCode", "ShareHolder", "Title" },
                values: new object[] { 1, "3573330000005", "Y", "783755784", "T9330044821", 30, 30, "U", "K", "4683465", "P", "TEST" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Works");
        }
    }
}
