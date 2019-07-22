using Microsoft.EntityFrameworkCore.Migrations;

namespace VH.PluralsightScraper.Migrations
{
    public partial class ChannelCourseUniqueIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ChannelCourse_ChannelId",
                schema: "public",
                table: "ChannelCourse");

            migrationBuilder.CreateIndex(
                name: "IX_ChannelCourse_ChannelId_CourseId",
                schema: "public",
                table: "ChannelCourse",
                columns: new[] { "ChannelId", "CourseId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ChannelCourse_ChannelId_CourseId",
                schema: "public",
                table: "ChannelCourse");

            migrationBuilder.CreateIndex(
                name: "IX_ChannelCourse_ChannelId",
                schema: "public",
                table: "ChannelCourse",
                column: "ChannelId");
        }
    }
}
