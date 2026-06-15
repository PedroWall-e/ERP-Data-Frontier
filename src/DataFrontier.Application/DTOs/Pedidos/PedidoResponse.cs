namespace DataFrontier.Application.DTOs.Pedidos;

public class PedidoResponse
{
    public Guid Id { get; set; }
    public string NumeroPedido { get; set; } = string.Empty;
    public DateTime DataEmissao { get; set; }
    public string Status { get; set; } = string.Empty;
    public Guid PessoaId { get; set; }
    public string PessoaNome { get; set; } = string.Empty;
    public string PessoaCpfCnpj { get; set; } = string.Empty;
    public decimal ValorTotalProdutos { get; set; }
    public decimal ValorTotalDesconto { get; set; }
    public decimal ValorTotalImpostos { get; set; }
    public decimal ValorTotalPedido { get; set; }
    public string? Observacoes { get; set; }
    public int? NumeroNfe { get; set; }
    public string? ChaveAcessoNfe { get; set; }
    public string? CaminhoPdfDanfe { get; set; }
    public long? NumeroNfse { get; set; }
    public string? CodigoVerificacaoNfse { get; set; }
    public string? LinkNfseNacional { get; set; }
    public List<PedidoItemResponse> Itens { get; set; } = new();
    public DateTime CriadoEm { get; set; }
}

public class PedidoItemResponse
{
    public Guid Id { get; set; }
    public Guid ProdutoId { get; set; }
    public string ProdutoNome { get; set; } = string.Empty;
    public string ProdutoCodigo { get; set; } = string.Empty;
    public decimal Quantidade { get; set; }
    public decimal ValorUnitario { get; set; }
    public decimal ValorDesconto { get; set; }
    public decimal ValorTotal { get; set; }
    public List<PedidoItemImpostoResponse> Impostos { get; set; } = new();
}

public class PedidoItemImpostoResponse
{
    public Guid Id { get; set; }
    public string NomeImposto { get; set; } = string.Empty;
    public decimal BaseCalculo { get; set; }
    public decimal Aliquota { get; set; }
    public decimal ValorImposto { get; set; }
}
