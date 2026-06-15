namespace DataFrontier.Application.DTOs.Empresa;

public class ConfiguracaoEmpresaRequest
{
    public string RazaoSocial { get; set; } = string.Empty;
    public string NomeFantasia { get; set; } = string.Empty;
    public string Cnpj { get; set; } = string.Empty;
    public string InscricaoEstadual { get; set; } = string.Empty;
    public string? InscricaoMunicipal { get; set; }
    public int CRT { get; set; } = 3;

    public string Logradouro { get; set; } = string.Empty;
    public string Numero { get; set; } = string.Empty;
    public string? Complemento { get; set; }
    public string Bairro { get; set; } = string.Empty;
    public string CodigoIbge { get; set; } = string.Empty;
    public string Municipio { get; set; } = string.Empty;
    public string Uf { get; set; } = string.Empty;
    public string Cep { get; set; } = string.Empty;
    public string? Telefone { get; set; }
    public string? Email { get; set; }

    public int AmbienteFiscal { get; set; } = 2;
    public int SerieNfe { get; set; } = 1;
    public int SerieNfse { get; set; } = 1;

    public string? InterClientId { get; set; }
    public string? InterClientSecret { get; set; }
}
