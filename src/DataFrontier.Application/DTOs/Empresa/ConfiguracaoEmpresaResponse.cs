namespace DataFrontier.Application.DTOs.Empresa;

public class ConfiguracaoEmpresaResponse
{
    public Guid Id { get; set; }

    public string RazaoSocial { get; set; } = string.Empty;
    public string NomeFantasia { get; set; } = string.Empty;
    public string Cnpj { get; set; } = string.Empty;
    public string InscricaoEstadual { get; set; } = string.Empty;
    public string? InscricaoMunicipal { get; set; }
    public int CRT { get; set; }

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

    // Certificado — nunca retorna o binário, apenas metadata
    public bool TemCertificado { get; set; }
    public string? CertificadoNome { get; set; }
    public DateTime? CertificadoValidade { get; set; }
    public bool CertificadoExpirado => CertificadoValidade.HasValue && CertificadoValidade.Value < DateTime.UtcNow;

    public int AmbienteFiscal { get; set; }
    public string AmbienteLabel => AmbienteFiscal == 1 ? "Produção" : "Homologação";
    public int SerieNfe { get; set; }
    public int SerieNfse { get; set; }

    // Inter — parcialmente mascarado
    public bool TemInterConfig { get; set; }
    public string? InterClientId { get; set; }

    public DateTime CriadoEm { get; set; }
    public DateTime? AtualizadoEm { get; set; }
}
