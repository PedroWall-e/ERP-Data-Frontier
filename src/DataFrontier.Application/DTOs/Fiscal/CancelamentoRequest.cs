using System.ComponentModel.DataAnnotations;

namespace DataFrontier.Application.DTOs.Fiscal;

public class CancelamentoRequest
{
    [Required]
    [MinLength(15, ErrorMessage = "Justificativa deve ter no mínimo 15 caracteres.")]
    [MaxLength(255)]
    public string Justificativa { get; set; } = string.Empty;
}

public class CartaCorrecaoRequest
{
    [Required]
    [MinLength(15, ErrorMessage = "Texto da correção deve ter no mínimo 15 caracteres.")]
    [MaxLength(1000)]
    public string Texto { get; set; } = string.Empty;
}

public class InutilizacaoRequest
{
    public int Serie { get; set; } = 1;

    [Range(1, int.MaxValue)]
    public int NumeroInicio { get; set; }

    [Range(1, int.MaxValue)]
    public int NumeroFim { get; set; }

    [Required]
    [MinLength(15)]
    [MaxLength(255)]
    public string Justificativa { get; set; } = string.Empty;
}
