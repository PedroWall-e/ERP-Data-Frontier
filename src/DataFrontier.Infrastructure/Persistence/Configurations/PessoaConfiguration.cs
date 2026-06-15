using DataFrontier.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataFrontier.Infrastructure.Persistence.Configurations;

public class PessoaConfiguration : IEntityTypeConfiguration<Pessoa>
{
    public void Configure(EntityTypeBuilder<Pessoa> builder)
    {
        builder.ToTable("pessoas");

        builder.Property(p => p.TipoPessoa)
            .HasConversion<string>()
            .HasMaxLength(10)
            .HasColumnName("tipo_pessoa");

        builder.Property(p => p.CpfCnpj)
            .HasMaxLength(14)
            .HasColumnName("cpf_cnpj");

        builder.Property(p => p.RazaoSocial)
            .HasMaxLength(300)
            .HasColumnName("razao_social");

        builder.Property(p => p.NomeFantasia)
            .HasMaxLength(300)
            .HasColumnName("nome_fantasia");

        builder.Property(p => p.InscricaoEstadual)
            .HasMaxLength(20)
            .HasColumnName("inscricao_estadual");

        builder.Property(p => p.IndicadorIE)
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasColumnName("indicador_ie");

        builder.Property(p => p.IsCliente).HasColumnName("is_cliente");
        builder.Property(p => p.IsFornecedor).HasColumnName("is_fornecedor");
        builder.Property(p => p.Email).HasMaxLength(256).HasColumnName("email");
        builder.Property(p => p.Telefone).HasMaxLength(20).HasColumnName("telefone");
        builder.Property(p => p.Ativo).HasDefaultValue(true).HasColumnName("ativo");

        builder.HasIndex(p => new { p.TenantId, p.CpfCnpj })
            .IsUnique()
            .HasDatabaseName("ix_pessoas_tenant_cpfcnpj");

        builder.HasIndex(p => new { p.TenantId, p.RazaoSocial })
            .HasDatabaseName("ix_pessoas_tenant_razao");

        builder.HasOne(p => p.Endereco)
            .WithOne(e => e.Pessoa)
            .HasForeignKey<Endereco>(e => e.PessoaId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
