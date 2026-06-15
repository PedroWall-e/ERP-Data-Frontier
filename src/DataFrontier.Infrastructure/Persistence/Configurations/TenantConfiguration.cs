using DataFrontier.Domain.Entities;
using DataFrontier.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataFrontier.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuração Fluent API para a entidade <see cref="Tenant"/>.
/// Nota: Tenant NÃO possui Global Query Filter por TenantId
/// pois é a própria entidade raiz do multi-tenancy.
/// </summary>
public class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.ToTable("tenants");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(t => t.Nome)
            .HasColumnName("nome")
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(t => t.Cnpj)
            .HasColumnName("cnpj")
            .HasMaxLength(14)
            .IsRequired();

        builder.Property(t => t.Tier)
            .HasColumnName("tier")
            .HasConversion<string>()
            .HasMaxLength(15)
            .IsRequired();

        builder.Property(t => t.Ativo)
            .HasColumnName("ativo")
            .HasDefaultValue(true);

        builder.Property(t => t.CriadoEm)
            .HasColumnName("criado_em");

        builder.Property(t => t.AtualizadoEm)
            .HasColumnName("atualizado_em");

        // CNPJ único globalmente
        builder.HasIndex(t => t.Cnpj)
            .IsUnique()
            .HasDatabaseName("ix_tenants_cnpj");
    }
}
