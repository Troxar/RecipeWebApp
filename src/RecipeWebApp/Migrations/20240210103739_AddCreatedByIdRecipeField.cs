using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecipeWebApp.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatedByIdRecipeField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "Recipes",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Recipes");
        }
    }
}
