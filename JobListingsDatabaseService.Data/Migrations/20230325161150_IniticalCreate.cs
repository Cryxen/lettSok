using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobListingsDatabaseService.Data.Migrations
{
    /// <inheritdoc />
    public partial class IniticalCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Advertisement",
                columns: table => new
                {
                    Uuid = table.Column<string>(type: "varchar(255)", nullable: false),
                    Expires = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Municipal = table.Column<string>(type: "longtext", nullable: true),
                    Title = table.Column<string>(type: "longtext", nullable: true),
                    Description = table.Column<string>(type: "longtext", nullable: true),
                    JobTitle = table.Column<string>(type: "longtext", nullable: true),
                    Employer = table.Column<string>(type: "longtext", nullable: true),
                    EngagementType = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Advertisement", x => x.Uuid);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InterestedAdvertisement");

            migrationBuilder.DropTable(
                name: "UninterestedAdvertisement");

            migrationBuilder.DropTable(
                name: "Advertisement");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
