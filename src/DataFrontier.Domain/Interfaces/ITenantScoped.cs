namespace DataFrontier.Domain.Interfaces;

/// <summary>
/// Interface marker que identifica entidades com isolamento por tenant.
/// Todas as entidades que implementam esta interface recebem automaticamente
/// um Global Query Filter no Entity Framework Core, garantindo que cada
/// tenant acesse apenas seus próprios dados.
/// </summary>
public interface ITenantScoped
{
    /// <summary>
    /// Identificador único do tenant proprietário desta entidade.
    /// </summary>
    Guid TenantId { get; set; }
}
