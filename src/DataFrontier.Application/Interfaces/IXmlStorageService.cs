namespace DataFrontier.Application.Interfaces;

/// <summary>
/// Serviço de armazenamento de XMLs fiscais em disco local.
/// Estrutura: {basePath}/{tenantId}/{ano}/{mes}/{chave}-{tipo}.xml
/// Retenção mínima: 5 anos (obrigação legal).
/// </summary>
public interface IXmlStorageService
{
    Task<string> SalvarAsync(Guid tenantId, string chaveAcesso, string tipo, string xmlContent);
    Task<string?> ObterAsync(Guid tenantId, string chaveAcesso, string tipo);
    Task<byte[]?> ObterBytesAsync(Guid tenantId, string chaveAcesso, string tipo);
}
