using DataFrontier.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataFrontier.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuração Fluent API para a entidade <see cref="Produto"/>.
/// Define mapeamento de tabela, índices e constraints.
/// </summary>
public class ProdutoConfiguration : IEntityTypeConfiguration<Produto>
{
    public void Configure(EntityTypeBuilder<Produto> builder)
    {
        builder.ToTable("produtos");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(p => p.TenantId)
            .HasColumnName("tenant_id")
            .IsRequired();

        builder.Property(p => p.Nome)
            .HasColumnName("nome")
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(p => p.Descricao)
            .HasColumnName("descricao")
            .HasMaxLength(2000);

        builder.Property(p => p.Codigo)
            .HasColumnName("codigo")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(p => p.CodigoNcm)
            .HasColumnName("codigo_ncm")
            .HasMaxLength(8);

        builder.Property(p => p.PrecoUnitario)
            .HasColumnName("preco_unitario")
            .HasPrecision(18, 2);

        builder.Property(p => p.Ativo)
            .HasColumnName("ativo")
            .HasDefaultValue(true);

        builder.Property(p => p.CriadoEm)
            .HasColumnName("criado_em");

        builder.Property(p => p.AtualizadoEm)
            .HasColumnName("atualizado_em");

        // Campos de Serviço (NFS-e)
        builder.Property(p => p.IsServico).HasColumnName("is_servico").HasDefaultValue(false);
        builder.Property(p => p.CodigoServico).HasMaxLength(10).HasColumnName("codigo_servico");
        builder.Property(p => p.CodigoCnae).HasMaxLength(10).HasColumnName("codigo_cnae");
        builder.Property(p => p.CodigoNbs).HasMaxLength(15).HasColumnName("codigo_nbs");
        builder.Property(p => p.UnidadeMedida).HasMaxLength(5).HasColumnName("unidade_medida").HasDefaultValue("UN");

        // Índice composto para busca por tenant + código (unicidade por tenant)
        builder.HasIndex(p => new { p.TenantId, p.Codigo })
            .IsUnique()
            .HasDatabaseName("ix_produtos_tenant_codigo");

        // Índice para busca por tenant + nome
        builder.HasIndex(p => new { p.TenantId, p.Nome })
            .HasDatabaseName("ix_produtos_tenant_nome");
    }
}
