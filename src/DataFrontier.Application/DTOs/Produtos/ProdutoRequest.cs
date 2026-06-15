using System.ComponentModel.DataAnnotations;

namespace DataFrontier.Application.DTOs.Produtos;

/// <summary>
/// Dados para criação ou atualização de um produto.
/// </summary>
public class ProdutoRequest
{
    /// <summary>
    /// Nome comercial do produto.
    /// </summary>
    [Required(ErrorMessage = "Nome é obrigatório.")]
    [MaxLength(300)]
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Descrição detalhada do produto.
    /// </summary>
    [MaxLength(2000)]
    public string? Descricao { get; set; }

    /// <summary>
    /// Código interno (SKU).
    /// </summary>
    [Required(ErrorMessage = "Código é obrigatório.")]
    [MaxLength(50)]
    public string Codigo { get; set; } = string.Empty;

    /// <summary>
    /// Código NCM para classificação fiscal (8 dígitos).
    /// </summary>
    [MaxLength(8)]
    [RegularExpression(@"^\d{0,8}$", ErrorMessage = "NCM deve conter apenas dígitos (máx. 8).")]
    public string? CodigoNcm { get; set; }

    /// <summary>
    /// Preço unitário de venda em reais.
    /// </summary>
    [Range(0.01, 99999999.99, ErrorMessage = "Preço deve ser maior que zero.")]
    public decimal PrecoUnitario { get; set; }

    // ── Serviço ──────────────────────────────────────────────────
    public bool IsServico { get; set; } = false;

    [MaxLength(10)]
    public string? CodigoServico { get; set; }

    [MaxLength(10)]
    public string? CodigoCnae { get; set; }

    [MaxLength(15)]
    public string? CodigoNbs { get; set; }

    [MaxLength(5)]
    public string UnidadeMedida { get; set; } = "UN";
}
