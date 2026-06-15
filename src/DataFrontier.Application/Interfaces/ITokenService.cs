using DataFrontier.Domain.Entities;

namespace DataFrontier.Application.Interfaces;

/// <summary>
/// Contrato para geração de tokens JWT com claims de tenant.
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Gera um token JWT contendo as claims do usuário e do tenant.
    /// Claims incluídas: sub, email, name, tenant_id, tenant_tier.
    /// </summary>
    /// <param name="usuario">Usuário autenticado.</param>
    /// <param name="tenant">Tenant do usuário.</param>
    /// <returns>Token JWT assinado como string.</returns>
    string GenerateToken(Usuario usuario, Tenant tenant);

    /// <summary>
    /// Obtém a data de expiração do próximo token a ser gerado.
    /// </summary>
    DateTime GetExpiration();
}
