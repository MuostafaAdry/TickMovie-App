using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoviePoint.Migrations
{
    /// <inheritdoc />
    public partial class addOrderModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LoginVM",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RememberMe = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoginVM", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    MovieId = table.Column<int>(type: "int", nullable: false),
                    ApplicationUserId = table.Column<int>(type: "int", nullable: false),
                    ApplicationUserId1 = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Count = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => new { x.MovieId, x.ApplicationUserId });
                    table.ForeignKey(
                        name: "FK_Orders_AspNetUsers_ApplicationUserId1",
                        column: x => x.ApplicationUserId1,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Orders_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ApplicationUserId1",
                table: "Orders",
                column: "ApplicationUserId1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LoginVM");

            migrationBuilder.DropTable(
                name: "Orders");
        }
    }
}
