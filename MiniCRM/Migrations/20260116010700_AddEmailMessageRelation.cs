using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniCRM.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailMessageRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ContactId",
                table: "EmailMessages",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_EmailMessages_ContactId",
                table: "EmailMessages",
                column: "ContactId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmailMessages_Contacts_ContactId",
                table: "EmailMessages",
                column: "ContactId",
                principalTable: "Contacts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmailMessages_Contacts_ContactId",
                table: "EmailMessages");

            migrationBuilder.DropIndex(
                name: "IX_EmailMessages_ContactId",
                table: "EmailMessages");

            migrationBuilder.DropColumn(
                name: "ContactId",
                table: "EmailMessages");
        }
    }
}
