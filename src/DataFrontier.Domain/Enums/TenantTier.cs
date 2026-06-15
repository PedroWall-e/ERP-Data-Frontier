namespace DataFrontier.Domain.Enums;

/// <summary>
/// Modalidade de contratação do tenant, que determina a estratégia
/// de isolamento de dados utilizada.
/// </summary>
public enum TenantTier
{
    /// <summary>
    /// Banco de dados compartilhado com isolamento por linha (Global Query Filters).
    /// Custo reduzido, ideal para pequenas e médias empresas.
    /// </summary>
    SaaS = 1,

    /// <summary>
    /// Banco de dados físico isolado por tenant.
    /// Máximo isolamento e performance, ideal para grandes empresas.
    /// </summary>
    Enterprise = 2
}
