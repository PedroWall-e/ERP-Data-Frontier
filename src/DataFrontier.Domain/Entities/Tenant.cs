using DataFrontier.Domain.Enums;

namespace DataFrontier.Domain.Entities;

/// <summary>
/// Representa uma organização (empresa/tenant) cadastrada no sistema.
/// Esta é a entidade raiz do multi-tenancy — não implementa <see cref="Interfaces.ITenantScoped"/>
/// pois é a própria referência de isolamento.
/// 
/// Cada tenant possui sua própria modalidade de contratação (SaaS ou Enterprise),
/// que determina a estratégia de isolamento de dados utilizada.
/// </summary>
public class Tenant
{
    /// <summary>
    /// Identificador único do tenant.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Razão social ou nome fantasia da empresa.
    /// </summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// CNPJ da empresa. Formato: apenas dígitos (14 caracteres).
    /// </summary>
    public string Cnpj { get; set; } = string.Empty;

    /// <summary>
    /// Modalidade de contratação que determina o isolamento de dados.
    /// </summary>
    public TenantTier Tier { get; set; } = TenantTier.SaaS;

    /// <summary>
    /// Indica se o tenant está ativo no sistema.
    /// </summary>
    public bool Ativo { get; set; } = true;

    /// <summary>
    /// Data e hora UTC de criação do registro.
    /// </summary>
    public DateTime CriadoEm { get; set; }

    /// <summary>
    /// Data e hora UTC da última atualização.
    /// </summary>
    public DateTime? AtualizadoEm { get; set; }
}
