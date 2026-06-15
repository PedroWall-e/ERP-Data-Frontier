using DataFrontier.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataFrontier.Infrastructure.Persistence.Configurations;

public class PedidoItemConfiguration : IEntityTypeConfiguration<PedidoItem>
{
    public void Configure(EntityTypeBuilder<PedidoItem> builder)
    {
        builder.ToTable("pedido_itens");

        builder.Property(i => i.PedidoId).HasColumnName("pedido_id");
        builder.Property(i => i.ProdutoId).HasColumnName("produto_id");
        builder.Property(i => i.ProdutoNome).HasMaxLength(300).HasColumnName("produto_nome");
        builder.Property(i => i.ProdutoCodigo).HasMaxLength(50).HasColumnName("produto_codigo");
        builder.Property(i => i.Quantidade).HasPrecision(18, 4).HasColumnName("quantidade");
        builder.Property(i => i.ValorUnitario).HasPrecision(18, 2).HasColumnName("valor_unitario");
        builder.Property(i => i.ValorDesconto).HasPrecision(18, 2).HasColumnName("valor_desconto");
        builder.Property(i => i.ValorTotal).HasPrecision(18, 2).HasColumnName("valor_total");

        builder.HasOne(i => i.Produto).WithMany().HasForeignKey(i => i.ProdutoId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(i => i.Impostos).WithOne(imp => imp.PedidoItem).HasForeignKey(imp => imp.PedidoItemId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
