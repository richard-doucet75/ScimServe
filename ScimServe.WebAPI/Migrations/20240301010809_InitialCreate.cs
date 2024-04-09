using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScimServe.WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "scim");

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "scim",
                columns: table => new
                {
                    Key = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserNameLabelReservationToken = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    ExternalId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    RecordTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    RecordUserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsSystemUser = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Key);
                    table.UniqueConstraint("AK_Users_Id_Version", x => new { x.Id, x.Version });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users",
                schema: "scim");
        }
    }
}
