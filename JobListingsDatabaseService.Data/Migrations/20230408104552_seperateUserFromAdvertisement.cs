using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobListingsDatabaseService.Data.Migrations
{
    /// <inheritdoc />
    public partial class seperateUserFromAdvertisement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InterestedAdvertisement");

            migrationBuilder.DropTable(
                name: "UninterestedAdvertisement");

            migrationBuilder.DropTable(
                name: "User");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "InterestedAdvertisement",
                columns: table => new
                {
                    UserGuid = table.Column<Guid>(type: "char(36)", nullable: false),
                    AdvertisementUuid = table.Column<string>(type: "varchar(255)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterestedAdvertisement", x => new { x.UserGuid, x.AdvertisementUuid });
                    table.ForeignKey(
                        name: "FK_InterestedAdvertisement_Advertisement_AdvertisementUuid",
                        column: x => x.AdvertisementUuid,
                        principalTable: "Advertisement",
                        principalColumn: "Uuid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InterestedAdvertisement_User_UserGuid",
                        column: x => x.UserGuid,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "UninterestedAdvertisement",
                columns: table => new
                {
                    UserGuid = table.Column<Guid>(type: "char(36)", nullable: false),
                    AdvertisementUuid = table.Column<string>(type: "varchar(255)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UninterestedAdvertisement", x => new { x.UserGuid, x.AdvertisementUuid });
                    table.ForeignKey(
                        name: "FK_UninterestedAdvertisement_Advertisement_AdvertisementUuid",
                        column: x => x.AdvertisementUuid,
                        principalTable: "Advertisement",
                        principalColumn: "Uuid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UninterestedAdvertisement_User_UserGuid",
                        column: x => x.UserGuid,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_InterestedAdvertisement_AdvertisementUuid",
                table: "InterestedAdvertisement",
                column: "AdvertisementUuid");

            migrationBuilder.CreateIndex(
                name: "IX_UninterestedAdvertisement_AdvertisementUuid",
                table: "UninterestedAdvertisement",
                column: "AdvertisementUuid");
        }
    }
}
