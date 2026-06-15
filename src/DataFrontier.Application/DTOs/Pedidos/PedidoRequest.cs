using System.ComponentModel.DataAnnotations;

namespace DataFrontier.Application.DTOs.Pedidos;

public class PedidoRequest
{
    [Required]
    public Guid PessoaId { get; set; }

    [StringLength(2000)]
    public string? Observacoes { get; set; }

    [Required, MinLength(1, ErrorMessage = "O pedido deve ter ao menos um item.")]
    public List<PedidoItemRequest> Itens { get; set; } = new();
}

public class PedidoItemRequest
{
    [Required]
    public Guid ProdutoId { get; set; }

    [Range(0.0001, (double)decimal.MaxValue, ErrorMessage = "Quantidade deve ser maior que zero.")]
    public decimal Quantidade { get; set; }

    /// <summary>Se 0, usa o preço unitário do cadastro do produto.</summary>
    public decimal ValorUnitario { get; set; }

    public decimal ValorDesconto { get; set; }

    public List<PedidoItemImpostoRequest>? Impostos { get; set; }
}

public class PedidoItemImpostoRequest
{
    [Required, StringLength(20)]
    public string NomeImposto { get; set; } = string.Empty;

    public decimal BaseCalculo { get; set; }
    public decimal Aliquota { get; set; }
    public decimal ValorImposto { get; set; }
}

public class StatusPedidoRequest
{
    [Required]
    public string Status { get; set; } = string.Empty;
}
