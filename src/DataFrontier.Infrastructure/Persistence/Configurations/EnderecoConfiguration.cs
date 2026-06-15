using DataFrontier.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataFrontier.Infrastructure.Persistence.Configurations;

public class EnderecoConfiguration : IEntityTypeConfiguration<Endereco>
{
    public void Configure(EntityTypeBuilder<Endereco> builder)
    {
        builder.ToTable("enderecos");

        builder.Property(e => e.PessoaId).HasColumnName("pessoa_id");
        builder.Property(e => e.Logradouro).HasMaxLength(500).HasColumnName("logradouro");
        builder.Property(e => e.Numero).HasMaxLength(10).HasColumnName("numero");
        builder.Property(e => e.Complemento).HasMaxLength(200).HasColumnName("complemento");
        builder.Property(e => e.Bairro).HasMaxLength(200).HasColumnName("bairro");
        builder.Property(e => e.Cep).HasMaxLength(8).HasColumnName("cep");
        builder.Property(e => e.Cidade).HasMaxLength(200).HasColumnName("cidade");
        builder.Property(e => e.Uf).HasMaxLength(2).HasColumnName("uf");
        builder.Property(e => e.CodigoIbge).HasMaxLength(7).HasColumnName("codigo_ibge");
        builder.Property(e => e.Pais).HasMaxLength(100).HasDefaultValue("Brasil").HasColumnName("pais");
        builder.Property(e => e.CodigoPais).HasMaxLength(5).HasDefaultValue("1058").HasColumnName("codigo_pais");

        builder.HasIndex(e => e.PessoaId)
            .IsUnique()
            .HasDatabaseName("ix_enderecos_pessoa");
    }
}
