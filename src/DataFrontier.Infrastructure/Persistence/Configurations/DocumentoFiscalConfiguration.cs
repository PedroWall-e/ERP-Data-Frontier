using DataFrontier.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataFrontier.Infrastructure.Persistence.Configurations;

public class DocumentoFiscalConfiguration : IEntityTypeConfiguration<DocumentoFiscal>
{
    public void Configure(EntityTypeBuilder<DocumentoFiscal> builder)
    {
        builder.ToTable("documentos_fiscais");
        builder.HasKey(d => d.Id);
        builder.Property(d => d.Id).HasColumnName("id");
        builder.Property(d => d.TenantId).HasColumnName("tenant_id");
        builder.Property(d => d.CriadoEm).HasColumnName("criado_em");
        builder.Property(d => d.AtualizadoEm).HasColumnName("atualizado_em");

        builder.Property(d => d.PedidoId).HasColumnName("pedido_id");
        builder.Property(d => d.Tipo).HasConversion<string>().HasMaxLength(10).HasColumnName("tipo");
        builder.Property(d => d.ChaveAcesso).HasMaxLength(50).HasColumnName("chave_acesso");
        builder.Property(d => d.Numero).HasColumnName("numero");
        builder.Property(d => d.Serie).HasColumnName("serie");
        builder.Property(d => d.Protocolo).HasMaxLength(50).HasColumnName("protocolo");
        builder.Property(d => d.Status).HasConversion<string>().HasMaxLength(15).HasColumnName("status");
        builder.Property(d => d.DataAutorizacao).HasColumnName("data_autorizacao");
        builder.Property(d => d.CaminhoXml).HasMaxLength(500).HasColumnName("caminho_xml");
        builder.Property(d => d.CaminhoPdf).HasMaxLength(500).HasColumnName("caminho_pdf");
        builder.Property(d => d.XmlAutorizacao).HasColumnName("xml_autorizacao");

        builder.Property(d => d.ProtocoloCancelamento).HasMaxLength(50).HasColumnName("protocolo_cancelamento");
        builder.Property(d => d.JustificativaCancelamento).HasMaxLength(255).HasColumnName("justificativa_cancelamento");
        builder.Property(d => d.DataCancelamento).HasColumnName("data_cancelamento");

        builder.Property(d => d.TextoCartaCorrecao).HasMaxLength(1000).HasColumnName("texto_carta_correcao");
        builder.Property(d => d.SequenciaCartaCorrecao).HasColumnName("sequencia_carta_correcao");

        builder.HasOne(d => d.Pedido).WithMany().HasForeignKey(d => d.PedidoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(d => new { d.TenantId, d.ChaveAcesso })
            .HasDatabaseName("ix_documentos_fiscais_tenant_chave");
        builder.HasIndex(d => new { d.TenantId, d.Tipo, d.Numero })
            .HasDatabaseName("ix_documentos_fiscais_tenant_tipo_numero");
    }
}
