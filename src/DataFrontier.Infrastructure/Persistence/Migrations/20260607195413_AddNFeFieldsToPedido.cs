using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataFrontier.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddNFeFieldsToPedido : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "caminho_pdf_danfe",
                table: "pedidos",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "chave_acesso_nfe",
                table: "pedidos",
                type: "character varying(44)",
                maxLength: 44,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "numero_nfe",
                table: "pedidos",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "caminho_pdf_danfe",
                table: "pedidos");

            migrationBuilder.DropColumn(
                name: "chave_acesso_nfe",
                table: "pedidos");

            migrationBuilder.DropColumn(
                name: "numero_nfe",
                table: "pedidos");
        }
    }
}
