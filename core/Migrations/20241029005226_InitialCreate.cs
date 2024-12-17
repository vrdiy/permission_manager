using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PermissionManagerCore.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    PermMask = table.Column<byte[]>(type: "BLOB", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    PermMask = table.Column<byte[]>(type: "BLOB", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "User_Groups",
                columns: table => new
                {
                    User_Name = table.Column<string>(type: "TEXT", nullable: false),
                    Group_Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User_Groups", x => new { x.User_Name, x.Group_Name });
                    table.ForeignKey(
                        name: "FK_User_Groups_Groups_Group_Name",
                        column: x => x.Group_Name,
                        principalTable: "Groups",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_User_Groups_Users_User_Name",
                        column: x => x.User_Name,
                        principalTable: "Users",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_User_Groups_Group_Name",
                table: "User_Groups",
                column: "Group_Name");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "User_Groups");

            migrationBuilder.DropTable(
                name: "Groups");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
