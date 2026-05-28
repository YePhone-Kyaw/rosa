using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    public partial class RenameNameColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE `Products` CHANGE `Name` `ProductName` longtext NOT NULL;");
            migrationBuilder.Sql("ALTER TABLE `Categories` CHANGE `Name` `CategoryName` longtext NOT NULL;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE `Products` CHANGE `ProductName` `Name` longtext NOT NULL;");
            migrationBuilder.Sql("ALTER TABLE `Categories` CHANGE `CategoryName` `Name` longtext NOT NULL;");
        }
    }
}
