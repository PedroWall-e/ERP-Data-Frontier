using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataFrontier.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPessoas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "pessoas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    tipo_pessoa = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    cpf_cnpj = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: false),
                    razao_social = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    nome_fantasia = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    inscricao_estadual = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    indicador_ie = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    is_cliente = table.Column<bool>(type: "boolean", nullable: false),
                    is_fornecedor = table.Column<bool>(type: "boolean", nullable: false),
                    email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    telefone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pessoas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "enderecos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    pessoa_id = table.Column<Guid>(type: "uuid", nullable: false),
                    logradouro = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    numero = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    complemento = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    bairro = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    cep = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    cidade = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    uf = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    codigo_ibge = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: false),
                    pais = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false, defaultValue: "Brasil"),
                    codigo_pais = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false, defaultValue: "1058"),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_enderecos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_enderecos_pessoas_pessoa_id",
                        column: x => x.pessoa_id,
                        principalTable: "pessoas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_enderecos_pessoa",
                table: "enderecos",
                column: "pessoa_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_pessoas_tenant_cpfcnpj",
                table: "pessoas",
                columns: new[] { "TenantId", "cpf_cnpj" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_pessoas_tenant_razao",
                table: "pessoas",
                columns: new[] { "TenantId", "razao_social" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "enderecos");

            migrationBuilder.DropTable(
                name: "pessoas");
        }
    }
}
