namespace DataFrontier.Domain.Entities;

/// <summary>
/// Entidade de domínio que representa um produto ou serviço cadastrado no sistema.
/// Utilizada como exemplo de entidade com suporte completo a multi-tenancy.
/// </summary>
public class Produto : BaseEntity
{
    /// <summary>
    /// Nome comercial do produto.
    /// </summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Descrição detalhada do produto.
    /// </summary>
    public string? Descricao { get; set; }

    /// <summary>
    /// Código interno do produto (SKU).
    /// </summary>
    public string Codigo { get; set; } = string.Empty;

    /// <summary>
    /// Código NCM (Nomenclatura Comum do Mercosul) para classificação fiscal.
    /// Formato: 8 dígitos (ex: "85171231").
    /// </summary>
    public string? CodigoNcm { get; set; }

    /// <summary>
    /// Preço unitário de venda.
    /// </summary>
    public decimal PrecoUnitario { get; set; }

    /// <summary>
    /// Indica se o produto está ativo para comercialização.
    /// </summary>
    public bool Ativo { get; set; } = true;
}
