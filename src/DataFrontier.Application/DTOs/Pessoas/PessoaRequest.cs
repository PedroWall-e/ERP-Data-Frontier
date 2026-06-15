using System.ComponentModel.DataAnnotations;

namespace DataFrontier.Application.DTOs.Pessoas;

public class PessoaRequest
{
    [Required]
    public string TipoPessoa { get; set; } = string.Empty;

    [Required, StringLength(14, MinimumLength = 11)]
    public string CpfCnpj { get; set; } = string.Empty;

    [Required, StringLength(300)]
    public string RazaoSocial { get; set; } = string.Empty;

    [StringLength(300)]
    public string? NomeFantasia { get; set; }

    [StringLength(20)]
    public string? InscricaoEstadual { get; set; }

    [Required]
    public string IndicadorIE { get; set; } = "NaoContribuinte";

    public bool IsCliente { get; set; }
    public bool IsFornecedor { get; set; }

    [EmailAddress, StringLength(256)]
    public string? Email { get; set; }

    [StringLength(20)]
    public string? Telefone { get; set; }

    public EnderecoRequest? Endereco { get; set; }
}

public class EnderecoRequest
{
    [Required, StringLength(500)]
    public string Logradouro { get; set; } = string.Empty;

    [Required, StringLength(10)]
    public string Numero { get; set; } = string.Empty;

    [StringLength(200)]
    public string? Complemento { get; set; }

    [Required, StringLength(200)]
    public string Bairro { get; set; } = string.Empty;

    [Required, StringLength(8, MinimumLength = 8)]
    public string Cep { get; set; } = string.Empty;

    [Required, StringLength(200)]
    public string Cidade { get; set; } = string.Empty;

    [Required, StringLength(2, MinimumLength = 2)]
    public string Uf { get; set; } = string.Empty;

    [Required, StringLength(7, MinimumLength = 7)]
    public string CodigoIbge { get; set; } = string.Empty;
}
