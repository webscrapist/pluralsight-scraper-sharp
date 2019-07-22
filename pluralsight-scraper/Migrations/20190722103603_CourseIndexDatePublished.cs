using Microsoft.EntityFrameworkCore.Migrations;

namespace VH.PluralsightScraper.Migrations
{
    public partial class CourseIndexDatePublished : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Courses_Name",
                schema: "public",
                table: "Courses");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_Name_DatePublished",
                schema: "public",
                table: "Courses",
                columns: new[] { "Name", "DatePublished" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Courses_Name_DatePublished",
                schema: "public",
                table: "Courses");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_Name",
                schema: "public",
                table: "Courses",
                column: "Name",
                unique: true);
        }
    }
}
