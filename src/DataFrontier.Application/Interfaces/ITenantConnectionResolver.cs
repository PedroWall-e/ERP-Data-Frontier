using DataFrontier.Domain.Enums;

namespace DataFrontier.Application.Interfaces;

/// <summary>
/// Contrato para resolução da connection string do banco de dados
/// de acordo com o tenant e sua modalidade de contratação.
/// 
/// Estratégias de resolução:
/// - <see cref="TenantTier.SaaS"/>: Retorna a connection string do banco compartilhado.
///   O isolamento de dados é garantido por Global Query Filters no EF Core.
/// - <see cref="TenantTier.Enterprise"/>: Consulta o catálogo de tenants e retorna
///   a connection string do banco de dados físico dedicado ao tenant.
/// 
/// Lifetime: Scoped (uma instância por requisição HTTP).
/// </summary>
public interface ITenantConnectionResolver
{
    /// <summary>
    /// Resolve a connection string para o banco de dados do tenant especificado.
    /// </summary>
    /// <param name="tenantId">Identificador único do tenant.</param>
    /// <param name="tier">Modalidade de contratação que determina a estratégia de isolamento.</param>
    /// <returns>A connection string do PostgreSQL para o tenant.</returns>
    /// <exception cref="InvalidOperationException">
    /// Lançada quando não é possível encontrar a connection string para o tenant Enterprise.
    /// </exception>
    string ResolveConnectionString(Guid tenantId, TenantTier tier);
}
