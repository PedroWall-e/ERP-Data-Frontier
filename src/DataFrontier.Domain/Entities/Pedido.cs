using DataFrontier.Domain.Enums;

namespace DataFrontier.Domain.Entities;

public class Pedido : BaseEntity
{
    public string NumeroPedido { get; set; } = string.Empty;
    public DateTime DataEmissao { get; set; }
    public Guid PessoaId { get; set; }
    public StatusPedido Status { get; set; } = StatusPedido.Rascunho;
    public decimal ValorTotalProdutos { get; set; }
    public decimal ValorTotalDesconto { get; set; }
    public decimal ValorTotalImpostos { get; set; }
    public decimal ValorTotalPedido { get; set; }
    public string? Observacoes { get; set; }

    // ── NF-e (preenchidos após faturamento via ACBrLib) ──────────
    public int? NumeroNfe { get; set; }
    public string? ChaveAcessoNfe { get; set; }
    public string? CaminhoPdfDanfe { get; set; }

    // ── NFS-e (preenchidos após faturamento de serviço) ──────────
    public long? NumeroNfse { get; set; }
    public string? CodigoVerificacaoNfse { get; set; }
    public string? LinkNfseNacional { get; set; }

    // Navigation
    public Pessoa Pessoa { get; set; } = null!;
    public ICollection<PedidoItem> Itens { get; set; } = new List<PedidoItem>();
}
