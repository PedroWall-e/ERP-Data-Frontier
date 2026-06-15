using DataFrontier.Domain.Enums;

namespace DataFrontier.Domain.Interfaces;

/// <summary>
/// Contrato para resolução do contexto de tenant na requisição atual.
/// A implementação deve extrair as informações do tenant a partir do token JWT
/// presente no HttpContext da requisição.
/// 
/// Lifetime: Scoped (uma instância por requisição HTTP).
/// </summary>
public interface ITenantResolver
{
    /// <summary>
    /// Obtém o identificador único do tenant da requisição atual.
    /// </summary>
    /// <returns>O Guid do tenant extraído da claim 'tenant_id' do JWT.</returns>
    /// <exception cref="Exceptions.TenantResolutionException">
    /// Lançada quando não é possível resolver o tenant (claim ausente ou inválida).
    /// </exception>
    Guid GetTenantId();

    /// <summary>
    /// Obtém a modalidade de contratação do tenant (SaaS ou Enterprise).
    /// </summary>
    /// <returns>O tier do tenant extraído da claim 'tenant_tier' do JWT.</returns>
    TenantTier GetTenantTier();

    /// <summary>
    /// Indica se o contexto de tenant foi resolvido com sucesso para a requisição atual.
    /// Retorna false para requisições anônimas ou endpoints públicos.
    /// </summary>
    bool IsResolved { get; }
}
