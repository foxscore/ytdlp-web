using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ytdlp.Web.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Content",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Length = table.Column<float>(type: "REAL", nullable: true),
                    Size = table.Column<long>(type: "INTEGER", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    UploadDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ChannelName = table.Column<string>(type: "TEXT", nullable: false),
                    Source = table.Column<string>(type: "TEXT", nullable: false),
                    SourceMediaId = table.Column<string>(type: "TEXT", nullable: false),
                    RequestedUri = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    IsAdultContent = table.Column<bool>(type: "INTEGER", nullable: false),
                    AssetGuid = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false),
                    ThumbnailAssetGuid = table.Column<string>(type: "TEXT", maxLength: 32, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Content", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Content");
        }
    }
}
