using DataFrontier.Application.DTOs.Fiscal;

namespace DataFrontier.Application.Interfaces;

/// <summary>
/// Abstração para operações fiscais NF-e (Nota Fiscal Eletrônica).
/// </summary>
public interface IFiscalService : IDisposable
{
    Task<FiscalResponseDTO> EmitirNFeAsync(Guid pedidoId);
    Task<FiscalResponseDTO> CancelarNFeAsync(Guid pedidoId, string justificativa);
    Task<FiscalResponseDTO> CartaCorrecaoAsync(Guid pedidoId, string textoCorrecao);
    Task<FiscalResponseDTO> InutilizarNumeracaoAsync(int serie, int nInicio, int nFim, string justificativa);
    Task<FiscalResponseDTO> ConsultarNFeAsync(Guid pedidoId);
}
