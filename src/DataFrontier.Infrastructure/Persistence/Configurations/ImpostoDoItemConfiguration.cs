using DataFrontier.Domain.Entities;
using DataFrontier.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataFrontier.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuração Fluent API para a entidade <see cref="ImpostoDoItem"/>.
/// Define mapeamento para a tabela de abstração tributária, incluindo
/// o campo JSONB para metadados específicos de cada tributo.
/// </summary>
public class ImpostoDoItemConfiguration : IEntityTypeConfiguration<ImpostoDoItem>
{
    public void Configure(EntityTypeBuilder<ImpostoDoItem> builder)
    {
        builder.ToTable("impostos_do_item");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(i => i.TenantId)
            .HasColumnName("tenant_id")
            .IsRequired();

        builder.Property(i => i.ItemDocumentoFiscalId)
            .HasColumnName("item_documento_fiscal_id")
            .IsRequired();

        builder.Property(i => i.TipoImposto)
            .HasColumnName("tipo_imposto")
            .HasConversion<string>()
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(i => i.CodigoTributario)
            .HasColumnName("codigo_tributario")
            .HasMaxLength(10);

        builder.Property(i => i.BaseCalculo)
            .HasColumnName("base_calculo")
            .HasPrecision(18, 2);

        builder.Property(i => i.Aliquota)
            .HasColumnName("aliquota")
            .HasPrecision(8, 4);

        builder.Property(i => i.ValorImposto)
            .HasColumnName("valor_imposto")
            .HasPrecision(18, 2);

        builder.Property(i => i.Categoria)
            .HasColumnName("categoria")
            .HasConversion<string>()
            .HasMaxLength(15)
            .IsRequired();

        // JSONB no PostgreSQL para dados específicos de cada tributo
        builder.Property(i => i.MetadadosAdicionais)
            .HasColumnName("metadados_adicionais")
            .HasColumnType("jsonb");

        builder.Property(i => i.CriadoEm)
            .HasColumnName("criado_em");

        builder.Property(i => i.AtualizadoEm)
            .HasColumnName("atualizado_em");

        // Índice para busca por item do documento fiscal
        builder.HasIndex(i => new { i.TenantId, i.ItemDocumentoFiscalId })
            .HasDatabaseName("ix_impostos_tenant_item");

        // Índice para consultas por tipo de imposto (relatórios fiscais)
        builder.HasIndex(i => new { i.TenantId, i.TipoImposto })
            .HasDatabaseName("ix_impostos_tenant_tipo");
    }
}
