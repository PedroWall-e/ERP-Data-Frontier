using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataFrontier.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddConfiguracaoEmpresa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "impostos_do_item");

            migrationBuilder.CreateTable(
                name: "configuracoes_empresa",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    razao_social = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    nome_fantasia = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    cnpj = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    inscricao_estadual = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    inscricao_municipal = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    crt = table.Column<int>(type: "integer", nullable: false),
                    logradouro = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    numero = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    complemento = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    bairro = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    codigo_ibge = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: false),
                    municipio = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    uf = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    cep = table.Column<string>(type: "character varying(9)", maxLength: 9, nullable: false),
                    telefone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    certificado_pfx = table.Column<byte[]>(type: "bytea", nullable: true),
                    certificado_senha = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    certificado_validade = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    certificado_nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ambiente_fiscal = table.Column<int>(type: "integer", nullable: false),
                    serie_nfe = table.Column<int>(type: "integer", nullable: false),
                    serie_nfse = table.Column<int>(type: "integer", nullable: false),
                    inter_client_id = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    inter_client_secret = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    inter_certificado_pfx = table.Column<byte[]>(type: "bytea", nullable: true),
                    inter_certificado_senha = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_configuracoes_empresa", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_configuracoes_empresa_tenant",
                table: "configuracoes_empresa",
                column: "tenant_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "configuracoes_empresa");

            migrationBuilder.CreateTable(
                name: "impostos_do_item",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    aliquota = table.Column<decimal>(type: "numeric(8,4)", precision: 8, scale: 4, nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    base_calculo = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    categoria = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    codigo_tributario = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    item_documento_fiscal_id = table.Column<Guid>(type: "uuid", nullable: false),
                    metadados_adicionais = table.Column<string>(type: "jsonb", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tipo_imposto = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    valor_imposto = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_impostos_do_item", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_impostos_tenant_item",
                table: "impostos_do_item",
                columns: new[] { "tenant_id", "item_documento_fiscal_id" });

            migrationBuilder.CreateIndex(
                name: "ix_impostos_tenant_tipo",
                table: "impostos_do_item",
                columns: new[] { "tenant_id", "tipo_imposto" });
        }
    }
}
