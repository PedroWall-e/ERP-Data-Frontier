namespace DataFrontier.Infrastructure.Fiscal;

/// <summary>
/// Configurações da ACBrLib.NFSe para emissão de NFS-e Nacional (RFB).
/// Valores padrão são carregados do appsettings e sobrescritos pelos dados
/// da ConfiguracaoEmpresa em tempo de execução.
/// </summary>
public class ACBrNFSeConfig
{
    public int Ambiente { get; set; } = 2;
    public string Provedor { get; set; } = "Nacional";
    public string PathSchemas { get; set; } = @"C:\ACBrLib\Schemas\NFSe";
    public string PathXml { get; set; } = @"C:\ACBrNFSe\XML";
    public string PathPdf { get; set; } = @"C:\ACBrNFSe\PDF";
    public int Serie { get; set; } = 1;
}
