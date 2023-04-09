using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace UserPreferencesDatabaseService.Data.Migrations
{
    /// <inheritdoc />
    public partial class oneToMany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_interestedAdvertisements_User_AdvertisementUuid",
                table: "interestedAdvertisements");

            migrationBuilder.DropIndex(
                name: "IX_interestedAdvertisements_AdvertisementUuid",
                table: "interestedAdvertisements");

            migrationBuilder.RenameColumn(
                name: "InterestedAdvertisementId",
                table: "interestedAdvertisements",
                newName: "Id");

            migrationBuilder.AlterColumn<string>(
                name: "AdvertisementUuid",
                table: "interestedAdvertisements",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "char(36)");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "interestedAdvertisements",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "UninterestedAdvertisement",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false),
                    AdvertisementUuid = table.Column<string>(type: "longtext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UninterestedAdvertisement", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UninterestedAdvertisement_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_interestedAdvertisements_UserId",
                table: "interestedAdvertisements",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UninterestedAdvertisement_UserId",
                table: "UninterestedAdvertisement",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_interestedAdvertisements_User_UserId",
                table: "interestedAdvertisements",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_interestedAdvertisements_User_UserId",
                table: "interestedAdvertisements");

            migrationBuilder.DropTable(
                name: "UninterestedAdvertisement");

            migrationBuilder.DropIndex(
                name: "IX_interestedAdvertisements_UserId",
                table: "interestedAdvertisements");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "interestedAdvertisements");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "interestedAdvertisements",
                newName: "InterestedAdvertisementId");

            migrationBuilder.AlterColumn<Guid>(
                name: "AdvertisementUuid",
                table: "interestedAdvertisements",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext");

            migrationBuilder.CreateIndex(
                name: "IX_interestedAdvertisements_AdvertisementUuid",
                table: "interestedAdvertisements",
                column: "AdvertisementUuid");

            migrationBuilder.AddForeignKey(
                name: "FK_interestedAdvertisements_User_AdvertisementUuid",
                table: "interestedAdvertisements",
                column: "AdvertisementUuid",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
