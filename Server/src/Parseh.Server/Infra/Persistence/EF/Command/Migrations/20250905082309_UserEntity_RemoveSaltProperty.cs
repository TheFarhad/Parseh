using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Parseh.Server.Infra.Persistence.EF.Command.Migrations
{
    /// <inheritdoc />
    public partial class UserEntity_RemoveSaltProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Salt",
                table: "Users");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Salt",
                table: "Users",
                type: "varchar(200)",
                unicode: false,
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }
    }
}
