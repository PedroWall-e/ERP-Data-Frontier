namespace DataFrontier.Domain.Entities;

/// <summary>
/// Configurações da empresa emitente por tenant.
/// Substitui o hardcoding do appsettings.json para dados fiscais.
/// 1 registro por tenant.
/// </summary>
public class ConfiguracaoEmpresa : BaseEntity
{
    // ── Identificação ────────────────────────────────────────────
    public string RazaoSocial { get; set; } = string.Empty;
    public string NomeFantasia { get; set; } = string.Empty;
    public string Cnpj { get; set; } = string.Empty;
    public string InscricaoEstadual { get; set; } = string.Empty;
    public string? InscricaoMunicipal { get; set; }
    public int CRT { get; set; } = 3;

    // ── Endereço ─────────────────────────────────────────────────
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

    // ── Certificado Digital A1 ───────────────────────────────────
    public byte[]? CertificadoPfx { get; set; }
    public string? CertificadoSenha { get; set; }
    public DateTime? CertificadoValidade { get; set; }
    public string? CertificadoNome { get; set; }

    // ── Fiscal ───────────────────────────────────────────────────
    public int AmbienteFiscal { get; set; } = 2; // 1=Produção, 2=Homologação
    public int SerieNfe { get; set; } = 1;
    public int SerieNfse { get; set; } = 1;

    // ── Banco Inter (PIX) ───────────────────────────────────────
    public string? InterClientId { get; set; }
    public string? InterClientSecret { get; set; }
    public byte[]? InterCertificadoPfx { get; set; }
    public string? InterCertificadoSenha { get; set; }
}
