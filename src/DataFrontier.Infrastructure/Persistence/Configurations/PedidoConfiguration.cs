using DataFrontier.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataFrontier.Infrastructure.Persistence.Configurations;

public class PedidoConfiguration : IEntityTypeConfiguration<Pedido>
{
    public void Configure(EntityTypeBuilder<Pedido> builder)
    {
        builder.ToTable("pedidos");

        builder.Property(p => p.NumeroPedido).HasMaxLength(20).HasColumnName("numero_pedido");
        builder.Property(p => p.DataEmissao).HasColumnName("data_emissao");
        builder.Property(p => p.PessoaId).HasColumnName("pessoa_id");
        builder.Property(p => p.Status).HasConversion<string>().HasMaxLength(15).HasColumnName("status");
        builder.Property(p => p.ValorTotalProdutos).HasPrecision(18, 2).HasColumnName("valor_total_produtos");
        builder.Property(p => p.ValorTotalDesconto).HasPrecision(18, 2).HasColumnName("valor_total_desconto");
        builder.Property(p => p.ValorTotalImpostos).HasPrecision(18, 2).HasColumnName("valor_total_impostos");
        builder.Property(p => p.ValorTotalPedido).HasPrecision(18, 2).HasColumnName("valor_total_pedido");
        builder.Property(p => p.Observacoes).HasMaxLength(2000).HasColumnName("observacoes");

        // NF-e
        builder.Property(p => p.NumeroNfe).HasColumnName("numero_nfe");
        builder.Property(p => p.ChaveAcessoNfe).HasMaxLength(44).HasColumnName("chave_acesso_nfe");
        builder.Property(p => p.CaminhoPdfDanfe).HasMaxLength(500).HasColumnName("caminho_pdf_danfe");

        builder.HasIndex(p => new { p.TenantId, p.NumeroPedido })
            .IsUnique().HasDatabaseName("ix_pedidos_tenant_numero");
        builder.HasIndex(p => new { p.TenantId, p.Status })
            .HasDatabaseName("ix_pedidos_tenant_status");
        builder.HasIndex(p => new { p.TenantId, p.DataEmissao })
            .HasDatabaseName("ix_pedidos_tenant_data");

        builder.HasOne(p => p.Pessoa).WithMany().HasForeignKey(p => p.PessoaId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(p => p.Itens).WithOne(i => i.Pedido).HasForeignKey(i => i.PedidoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
