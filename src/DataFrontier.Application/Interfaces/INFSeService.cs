using DataFrontier.Application.DTOs.Fiscal;

namespace DataFrontier.Application.Interfaces;

/// <summary>
/// Abstração para operações fiscais de NFS-e (Nota Fiscal de Serviço Eletrônica).
/// </summary>
public interface INFSeService : IDisposable
{
    /// <summary>
    /// Emite uma NFS-e a partir de um pedido confirmado (contendo serviços).
    /// </summary>
    Task<FiscalResponseDTO> EmitirNFSeAsync(Guid pedidoId);
}
