using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConsultantWebPage.Repo.Migrations
{
    /// <inheritdoc />
    public partial class FixForeignKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RealEstate_Cities_CitiesCityId",
                table: "RealEstate");

            migrationBuilder.DropForeignKey(
                name: "FK_RealEstate_RealEstateType_RealEstateTypeId1",
                table: "RealEstate");

            migrationBuilder.DropForeignKey(
                name: "FK_RealEstateCategories_Category_CategoriesId",
                table: "RealEstateCategories");

            migrationBuilder.DropIndex(
                name: "IX_RealEstate_CitiesCityId",
                table: "RealEstate");

            migrationBuilder.DropIndex(
                name: "IX_RealEstate_RealEstateTypeId1",
                table: "RealEstate");

            migrationBuilder.DropColumn(
                name: "CitiesCityId",
                table: "RealEstate");

            migrationBuilder.DropColumn(
                name: "RealEstateTypeId1",
                table: "RealEstate");

            migrationBuilder.RenameColumn(
                name: "SocialUrl_X",
                table: "AboutMe",
                newName: "SocialURL_X");

            migrationBuilder.RenameColumn(
                name: "SocialUrl_Linkedin",
                table: "AboutMe",
                newName: "SocialURL_LinkedIn");

            migrationBuilder.RenameColumn(
                name: "SocialUrl_Instagram",
                table: "AboutMe",
                newName: "SocialURL_Instagram");

            migrationBuilder.RenameColumn(
                name: "SocialUrl_Facebook",
                table: "AboutMe",
                newName: "SocialURL_Facebook");

            migrationBuilder.AlterColumn<string>(
                name: "MediaType",
                table: "RealEstateMedia",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "image",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<bool>(
                name: "IsMain",
                table: "RealEstateMedia",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AddForeignKey(
                name: "FK_RealEstateCategories_Category_CategoriesId",
                table: "RealEstateCategories",
                column: "CategoriesId",
                principalTable: "Category",
                principalColumn: "CategoriesId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RealEstateCategories_Category_CategoriesId",
                table: "RealEstateCategories");

            migrationBuilder.RenameColumn(
                name: "SocialURL_X",
                table: "AboutMe",
                newName: "SocialUrl_X");

            migrationBuilder.RenameColumn(
                name: "SocialURL_LinkedIn",
                table: "AboutMe",
                newName: "SocialUrl_Linkedin");

            migrationBuilder.RenameColumn(
                name: "SocialURL_Instagram",
                table: "AboutMe",
                newName: "SocialUrl_Instagram");

            migrationBuilder.RenameColumn(
                name: "SocialURL_Facebook",
                table: "AboutMe",
                newName: "SocialUrl_Facebook");

            migrationBuilder.AlterColumn<string>(
                name: "MediaType",
                table: "RealEstateMedia",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldDefaultValue: "image");

            migrationBuilder.AlterColumn<bool>(
                name: "IsMain",
                table: "RealEstateMedia",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "CitiesCityId",
                table: "RealEstate",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RealEstateTypeId1",
                table: "RealEstate",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RealEstate_CitiesCityId",
                table: "RealEstate",
                column: "CitiesCityId");

            migrationBuilder.CreateIndex(
                name: "IX_RealEstate_RealEstateTypeId1",
                table: "RealEstate",
                column: "RealEstateTypeId1");

            migrationBuilder.AddForeignKey(
                name: "FK_RealEstate_Cities_CitiesCityId",
                table: "RealEstate",
                column: "CitiesCityId",
                principalTable: "Cities",
                principalColumn: "CityId");

            migrationBuilder.AddForeignKey(
                name: "FK_RealEstate_RealEstateType_RealEstateTypeId1",
                table: "RealEstate",
                column: "RealEstateTypeId1",
                principalTable: "RealEstateType",
                principalColumn: "RealEstateTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_RealEstateCategories_Category_CategoriesId",
                table: "RealEstateCategories",
                column: "CategoriesId",
                principalTable: "Category",
                principalColumn: "CategoriesId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
