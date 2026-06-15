namespace DataFrontier.Application.DTOs.Pessoas;

public class PessoaResponse
{
    public Guid Id { get; set; }
    public string TipoPessoa { get; set; } = string.Empty;
    public string CpfCnpj { get; set; } = string.Empty;
    public string RazaoSocial { get; set; } = string.Empty;
    public string? NomeFantasia { get; set; }
    public string? InscricaoEstadual { get; set; }
    public string IndicadorIE { get; set; } = string.Empty;
    public bool IsCliente { get; set; }
    public bool IsFornecedor { get; set; }
    public string? Email { get; set; }
    public string? Telefone { get; set; }
    public bool Ativo { get; set; }
    public EnderecoResponse? Endereco { get; set; }
    public DateTime CriadoEm { get; set; }
    public DateTime? AtualizadoEm { get; set; }
}

public class EnderecoResponse
{
    public Guid Id { get; set; }
    public string Logradouro { get; set; } = string.Empty;
    public string Numero { get; set; } = string.Empty;
    public string? Complemento { get; set; }
    public string Bairro { get; set; } = string.Empty;
    public string Cep { get; set; } = string.Empty;
    public string Cidade { get; set; } = string.Empty;
    public string Uf { get; set; } = string.Empty;
    public string CodigoIbge { get; set; } = string.Empty;
}
