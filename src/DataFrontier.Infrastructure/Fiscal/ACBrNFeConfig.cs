namespace DataFrontier.Infrastructure.Fiscal;

/// <summary>
/// Configurações da ACBrLib.NFe mapeadas do appsettings.json (seção "ACBrNFe").
/// </summary>
public class ACBrNFeConfig
{
    /// <summary>1 = Produção, 2 = Homologação (padrão SEFAZ).</summary>
    public int Ambiente { get; set; } = 2;

    public string PathSchemas { get; set; } = @"C:\ACBrLib\Schemas\NFe";
    public string PathXml { get; set; } = @"C:\ACBrNFe\XML";
    public string PathPdf { get; set; } = @"C:\ACBrNFe\PDF";

    public CertificadoConfig Certificado { get; set; } = new();
    public EmitenteConfig Emitente { get; set; } = new();

    public int Serie { get; set; } = 1;
}

public class CertificadoConfig
{
    /// <summary>Caminho absoluto para o arquivo .pfx do certificado A1.</summary>
    public string Caminho { get; set; } = string.Empty;

    /// <summary>Senha do certificado A1.</summary>
    public string Senha { get; set; } = string.Empty;
}

public class EmitenteConfig
{
    public string CNPJ { get; set; } = string.Empty;
    public string IE { get; set; } = string.Empty;
    public string RazaoSocial { get; set; } = string.Empty;
    public string NomeFantasia { get; set; } = string.Empty;

    /// <summary>Código do Regime Tributário: 1=Simples, 2=SN Excesso, 3=Normal.</summary>
    public int CRT { get; set; } = 3;

    public string Logradouro { get; set; } = string.Empty;
    public string Numero { get; set; } = string.Empty;
    public string Complemento { get; set; } = string.Empty;
    public string Bairro { get; set; } = string.Empty;
    public string CodigoIBGE { get; set; } = string.Empty;
    public string Municipio { get; set; } = string.Empty;
    public string UF { get; set; } = string.Empty;
    public string CEP { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
}
