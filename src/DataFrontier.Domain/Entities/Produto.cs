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

    // ── Campos para Serviço (NFS-e Nacional) ─────────────────────
    /// <summary>Indica se o produto é um serviço (para NFS-e).</summary>
    public bool IsServico { get; set; } = false;

    /// <summary>Código da Lista de Serviços (LC 116/2003). Ex: "01.01"</summary>
    public string? CodigoServico { get; set; }

    /// <summary>Código CNAE da atividade. Ex: "6201501"</summary>
    public string? CodigoCnae { get; set; }

    /// <summary>NBS - Nomenclatura Brasileira de Serviços. Ex: "1.0101.0100"</summary>
    public string? CodigoNbs { get; set; }

    /// <summary>Unidade de medida (UN, HR, MES, etc.)</summary>
    public string UnidadeMedida { get; set; } = "UN";
}
