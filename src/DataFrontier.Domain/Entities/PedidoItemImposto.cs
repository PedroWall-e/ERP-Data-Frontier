namespace DataFrontier.Domain.Entities;

/// <summary>
/// Imposto vinculado a um item do pedido.
/// Estrutura flexível para suportar a transição da Reforma Tributária:
/// ICMS/PIS/COFINS (regime atual) e IBS/CBS (regime novo) podem coexistir.
/// </summary>
public class PedidoItemImposto : BaseEntity
{
    public Guid PedidoItemId { get; set; }
    public string NomeImposto { get; set; } = string.Empty;
    public decimal BaseCalculo { get; set; }
    public decimal Aliquota { get; set; }
    public decimal ValorImposto { get; set; }

    // Navigation
    public PedidoItem PedidoItem { get; set; } = null!;
}
