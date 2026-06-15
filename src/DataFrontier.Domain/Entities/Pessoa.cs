using DataFrontier.Domain.Enums;

namespace DataFrontier.Domain.Entities;

/// <summary>
/// Entidade que representa uma Pessoa (Cliente e/ou Fornecedor).
/// Contém todos os dados necessários para emissão de NF-e via ACBr.
/// </summary>
public class Pessoa : BaseEntity
{
    public TipoPessoa TipoPessoa { get; set; }
    public string CpfCnpj { get; set; } = string.Empty;
    public string RazaoSocial { get; set; } = string.Empty;
    public string? NomeFantasia { get; set; }
    public string? InscricaoEstadual { get; set; }
    public IndicadorIE IndicadorIE { get; set; } = IndicadorIE.NaoContribuinte;
    public bool IsCliente { get; set; }
    public bool IsFornecedor { get; set; }
    public string? Email { get; set; }
    public string? Telefone { get; set; }
    public bool Ativo { get; set; } = true;

    // Navigation — Endereço 1:1
    public Endereco? Endereco { get; set; }
}
