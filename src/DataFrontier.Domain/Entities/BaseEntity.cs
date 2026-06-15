using DataFrontier.Domain.Interfaces;

namespace DataFrontier.Domain.Entities;

/// <summary>
/// Classe base abstrata para todas as entidades do domínio com suporte a multi-tenancy.
/// Fornece propriedades padrão de identificação, isolamento por tenant e auditoria.
/// 
/// Todas as entidades de negócio devem herdar desta classe para garantir:
/// - Isolamento automático por tenant via Global Query Filters
/// - Rastreabilidade de criação e atualização
/// - Identificação única via GUID
/// </summary>
public abstract class BaseEntity : ITenantScoped
{
    /// <summary>
    /// Identificador único da entidade. Gerado automaticamente como GUID v7 (ordenável por tempo).
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Identificador do tenant proprietário. Populado automaticamente pelo
    /// <see cref="ITenantScoped"/> interceptor durante a persistência.
    /// </summary>
    public Guid TenantId { get; set; }

    /// <summary>
    /// Data e hora UTC de criação do registro. Populado automaticamente
    /// pelo AuditInterceptor.
    /// </summary>
    public DateTime CriadoEm { get; set; }

    /// <summary>
    /// Data e hora UTC da última atualização do registro. Null se nunca atualizado.
    /// Populado automaticamente pelo AuditInterceptor.
    /// </summary>
    public DateTime? AtualizadoEm { get; set; }
}
