namespace DataFrontier.Application.DTOs.Produtos;

/// <summary>
/// Dados de resposta de um produto.
/// </summary>
public class ProdutoResponse
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string? CodigoNcm { get; set; }
    public decimal PrecoUnitario { get; set; }
    public bool Ativo { get; set; }
    public DateTime CriadoEm { get; set; }
    public DateTime? AtualizadoEm { get; set; }
}
