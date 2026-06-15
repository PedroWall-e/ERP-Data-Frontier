namespace DataFrontier.Domain.Entities;

/// <summary>
/// Endereço vinculado a uma Pessoa.
/// Campos mapeados para os requisitos da NF-e (ACBr):
/// - CodigoIbge (cMunFG / cMunDest): código IBGE do município (7 dígitos)
/// - CodigoPais (cPais): código do país (1058 = Brasil)
/// </summary>
public class Endereco : BaseEntity
{
    public Guid PessoaId { get; set; }
    public string Logradouro { get; set; } = string.Empty;
    public string Numero { get; set; } = string.Empty;
    public string? Complemento { get; set; }
    public string Bairro { get; set; } = string.Empty;
    public string Cep { get; set; } = string.Empty;
    public string Cidade { get; set; } = string.Empty;
    public string Uf { get; set; } = string.Empty;
    public string CodigoIbge { get; set; } = string.Empty;
    public string Pais { get; set; } = "Brasil";
    public string CodigoPais { get; set; } = "1058";

    // Navigation
    public Pessoa Pessoa { get; set; } = null!;
}
