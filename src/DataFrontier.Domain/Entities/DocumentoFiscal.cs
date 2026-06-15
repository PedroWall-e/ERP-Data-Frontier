using DataFrontier.Domain.Enums;

namespace DataFrontier.Domain.Entities;

/// <summary>
/// Rastreia documentos fiscais emitidos (NF-e e NFS-e) com dados de protocolo,
/// cancelamento, carta de correção e guarda de XML.
/// </summary>
public class DocumentoFiscal : BaseEntity
{
    public Guid PedidoId { get; set; }
    public TipoDocumentoFiscal Tipo { get; set; }
    public string ChaveAcesso { get; set; } = string.Empty;
    public int Numero { get; set; }
    public int Serie { get; set; }
    public string? Protocolo { get; set; }
    public StatusDocumentoFiscal Status { get; set; } = StatusDocumentoFiscal.Autorizado;
    public DateTime DataAutorizacao { get; set; }
    public string? CaminhoXml { get; set; }
    public string? CaminhoPdf { get; set; }
    public string? XmlAutorizacao { get; set; }

    // Cancelamento
    public string? ProtocoloCancelamento { get; set; }
    public string? JustificativaCancelamento { get; set; }
    public DateTime? DataCancelamento { get; set; }

    // CC-e (Carta de Correção)
    public string? TextoCartaCorrecao { get; set; }
    public int SequenciaCartaCorrecao { get; set; }

    // Navigation
    public Pedido Pedido { get; set; } = null!;
}
