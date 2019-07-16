using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VH.PluralsightScraper.Migrations
{
    public partial class CourseDatePublishedNullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "DatePublished",
                schema: "public",
                table: "Courses",
                nullable: true,
                oldClrType: typeof(DateTime));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "DatePublished",
                schema: "public",
                table: "Courses",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);
        }
    }
}
