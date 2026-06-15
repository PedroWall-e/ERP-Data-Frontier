using DataFrontier.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataFrontier.Infrastructure.Persistence.Configurations;

public class PedidoItemImpostoConfiguration : IEntityTypeConfiguration<PedidoItemImposto>
{
    public void Configure(EntityTypeBuilder<PedidoItemImposto> builder)
    {
        builder.ToTable("pedido_item_impostos");

        builder.Property(i => i.PedidoItemId).HasColumnName("pedido_item_id");
        builder.Property(i => i.NomeImposto).HasMaxLength(20).HasColumnName("nome_imposto");
        builder.Property(i => i.BaseCalculo).HasPrecision(18, 2).HasColumnName("base_calculo");
        builder.Property(i => i.Aliquota).HasPrecision(8, 4).HasColumnName("aliquota");
        builder.Property(i => i.ValorImposto).HasPrecision(18, 2).HasColumnName("valor_imposto");

        builder.HasIndex(i => new { i.PedidoItemId, i.NomeImposto })
            .HasDatabaseName("ix_pedido_item_impostos_item_nome");
    }
}
