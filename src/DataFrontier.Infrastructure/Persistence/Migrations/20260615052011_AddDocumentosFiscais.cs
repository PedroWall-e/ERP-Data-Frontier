using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataFrontier.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDocumentosFiscais : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "documentos_fiscais",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    pedido_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tipo = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    chave_acesso = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    numero = table.Column<int>(type: "integer", nullable: false),
                    serie = table.Column<int>(type: "integer", nullable: false),
                    protocolo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    status = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    data_autorizacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    caminho_xml = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    caminho_pdf = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    xml_autorizacao = table.Column<string>(type: "text", nullable: true),
                    protocolo_cancelamento = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    justificativa_cancelamento = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    data_cancelamento = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    texto_carta_correcao = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    sequencia_carta_correcao = table.Column<int>(type: "integer", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_documentos_fiscais", x => x.id);
                    table.ForeignKey(
                        name: "FK_documentos_fiscais_pedidos_pedido_id",
                        column: x => x.pedido_id,
                        principalTable: "pedidos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_documentos_fiscais_pedido_id",
                table: "documentos_fiscais",
                column: "pedido_id");

            migrationBuilder.CreateIndex(
                name: "ix_documentos_fiscais_tenant_chave",
                table: "documentos_fiscais",
                columns: new[] { "tenant_id", "chave_acesso" });

            migrationBuilder.CreateIndex(
                name: "ix_documentos_fiscais_tenant_tipo_numero",
                table: "documentos_fiscais",
                columns: new[] { "tenant_id", "tipo", "numero" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "documentos_fiscais");
        }
    }
}
