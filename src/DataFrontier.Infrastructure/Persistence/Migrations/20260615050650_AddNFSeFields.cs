using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataFrontier.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddNFSeFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "codigo_cnae",
                table: "produtos",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "codigo_nbs",
                table: "produtos",
                type: "character varying(15)",
                maxLength: 15,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "codigo_servico",
                table: "produtos",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_servico",
                table: "produtos",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "unidade_medida",
                table: "produtos",
                type: "character varying(5)",
                maxLength: 5,
                nullable: false,
                defaultValue: "UN");

            migrationBuilder.AddColumn<string>(
                name: "codigo_verificacao_nfse",
                table: "pedidos",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "link_nfse_nacional",
                table: "pedidos",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "numero_nfse",
                table: "pedidos",
                type: "bigint",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "codigo_cnae",
                table: "produtos");

            migrationBuilder.DropColumn(
                name: "codigo_nbs",
                table: "produtos");

            migrationBuilder.DropColumn(
                name: "codigo_servico",
                table: "produtos");

            migrationBuilder.DropColumn(
                name: "is_servico",
                table: "produtos");

            migrationBuilder.DropColumn(
                name: "unidade_medida",
                table: "produtos");

            migrationBuilder.DropColumn(
                name: "codigo_verificacao_nfse",
                table: "pedidos");

            migrationBuilder.DropColumn(
                name: "link_nfse_nacional",
                table: "pedidos");

            migrationBuilder.DropColumn(
                name: "numero_nfse",
                table: "pedidos");
        }
    }
}
