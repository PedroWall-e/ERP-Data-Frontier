namespace DataFrontier.Domain.Entities;

public class PedidoItem : BaseEntity
{
    public Guid PedidoId { get; set; }
    public Guid ProdutoId { get; set; }
    public string ProdutoNome { get; set; } = string.Empty;
    public string ProdutoCodigo { get; set; } = string.Empty;
    public decimal Quantidade { get; set; }
    public decimal ValorUnitario { get; set; }
    public decimal ValorDesconto { get; set; }
    public decimal ValorTotal { get; set; }

    // Navigation
    public Pedido Pedido { get; set; } = null!;
    public Produto Produto { get; set; } = null!;
    public ICollection<PedidoItemImposto> Impostos { get; set; } = new List<PedidoItemImposto>();
}
