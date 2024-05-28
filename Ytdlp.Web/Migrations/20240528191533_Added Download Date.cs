using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ytdlp.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddedDownloadDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DownloadDate",
                table: "Content",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DownloadDate",
                table: "Content");
        }
    }
}
