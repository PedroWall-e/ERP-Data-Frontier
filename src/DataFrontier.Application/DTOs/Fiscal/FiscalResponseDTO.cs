namespace DataFrontier.Application.DTOs.Fiscal;

/// <summary>
/// Resposta padronizada da operação fiscal (emissão, cancelamento, etc.).
/// </summary>
public class FiscalResponseDTO
{
    public bool Sucesso { get; set; }
    public int? NumeroNfe { get; set; }
    public string? ChaveAcesso { get; set; }
    public string? Protocolo { get; set; }
    public string? CaminhoPdf { get; set; }
    public string? CaminhoXml { get; set; }
    public string? Mensagem { get; set; }
    public int? CodigoRetorno { get; set; }
    public string? IniGerado { get; set; }
}
