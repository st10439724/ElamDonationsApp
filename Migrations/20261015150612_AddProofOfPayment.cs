using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElamIgugu.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddProofOfPayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProofFileName",
                table: "Donations",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DonationProofs",
                columns: table => new
                {
                    DonationId = table.Column<int>(type: "int", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Data = table.Column<byte[]>(type: "varbinary(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DonationProofs", x => x.DonationId);
                    table.ForeignKey(
                        name: "FK_DonationProofs_Donations_DonationId",
                        column: x => x.DonationId,
                        principalTable: "Donations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DonationProofs");

            migrationBuilder.DropColumn(
                name: "ProofFileName",
                table: "Donations");
        }
    }
}
