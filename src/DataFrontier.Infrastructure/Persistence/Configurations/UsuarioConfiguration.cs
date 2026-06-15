using DataFrontier.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataFrontier.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuração Fluent API para a entidade <see cref="Usuario"/>.
/// </summary>
public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("usuarios");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(u => u.TenantId)
            .HasColumnName("tenant_id")
            .IsRequired();

        builder.Property(u => u.Email)
            .HasColumnName("email")
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(u => u.NomeCompleto)
            .HasColumnName("nome_completo")
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(u => u.SenhaHash)
            .HasColumnName("senha_hash")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(u => u.Ativo)
            .HasColumnName("ativo")
            .HasDefaultValue(true);

        builder.Property(u => u.UltimoLogin)
            .HasColumnName("ultimo_login");

        builder.Property(u => u.CriadoEm)
            .HasColumnName("criado_em");

        builder.Property(u => u.AtualizadoEm)
            .HasColumnName("atualizado_em");

        // E-mail único globalmente (cross-tenant)
        builder.HasIndex(u => u.Email)
            .IsUnique()
            .HasDatabaseName("ix_usuarios_email");

        // Índice para busca por tenant
        builder.HasIndex(u => u.TenantId)
            .HasDatabaseName("ix_usuarios_tenant");
    }
}
