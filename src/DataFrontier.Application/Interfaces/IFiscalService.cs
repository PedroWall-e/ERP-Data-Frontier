using DataFrontier.Application.DTOs.Fiscal;

namespace DataFrontier.Application.Interfaces;

/// <summary>
/// Abstração para operações fiscais (NF-e, NFC-e, etc.).
/// A implementação concreta utiliza ACBrLib.
/// </summary>
public interface IFiscalService : IDisposable
{
    /// <summary>
    /// Emite uma NF-e a partir de um pedido confirmado.
    /// Assina, transmite à SEFAZ, gera o PDF do DANFE e atualiza o pedido.
    /// </summary>
    Task<FiscalResponseDTO> EmitirNFeAsync(Guid pedidoId);
}
