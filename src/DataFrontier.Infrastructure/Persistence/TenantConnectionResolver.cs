using DataFrontier.Application.Interfaces;
using DataFrontier.Domain.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DataFrontier.Infrastructure.Persistence;

/// <summary>
/// Resolve a connection string do PostgreSQL de acordo com o tenant e sua modalidade.
/// 
/// Estratégias:
/// - SaaS: Retorna a connection string "SharedDatabase" do appsettings.
/// - Enterprise: Consulta a seção "MultiTenancy:EnterpriseTenants" usando o TenantId como chave.
/// 
/// Em iterações futuras, o catálogo Enterprise será migrado para uma tabela
/// TenantCatalog no banco administrativo, com cache distribuído.
/// </summary>
public class TenantConnectionResolver : ITenantConnectionResolver
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<TenantConnectionResolver> _logger;

    public TenantConnectionResolver(
        IConfiguration configuration,
        ILogger<TenantConnectionResolver> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    /// <inheritdoc/>
    public string ResolveConnectionString(Guid tenantId, TenantTier tier)
    {
        return tier switch
        {
            TenantTier.SaaS => ResolveSharedDatabase(),
            TenantTier.Enterprise => ResolveIsolatedDatabase(tenantId),
            _ => throw new InvalidOperationException(
                $"Modalidade de tenant não suportada: {tier}")
        };
    }

    /// <summary>
    /// Retorna a connection string do banco de dados compartilhado.
    /// </summary>
    private string ResolveSharedDatabase()
    {
        var connectionString = _configuration.GetConnectionString("SharedDatabase");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string 'SharedDatabase' não encontrada no appsettings. " +
                "Verifique a seção ConnectionStrings.");
        }

        return connectionString;
    }

    /// <summary>
    /// Consulta o catálogo de tenants Enterprise e retorna a connection string dedicada.
    /// </summary>
    private string ResolveIsolatedDatabase(Guid tenantId)
    {
        var tenantKey = tenantId.ToString();
        var connectionString = _configuration
            .GetSection($"MultiTenancy:EnterpriseTenants:{tenantKey}")
            .Value;

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            _logger.LogError(
                "Connection string não encontrada para tenant Enterprise {TenantId}. " +
                "Verifique a seção MultiTenancy:EnterpriseTenants no appsettings.",
                tenantId);

            throw new InvalidOperationException(
                $"Connection string não encontrada para o tenant Enterprise '{tenantId}'. " +
                $"O tenant pode não estar registrado no catálogo.");
        }

        _logger.LogDebug(
            "Connection string resolvida para tenant Enterprise {TenantId}",
            tenantId);

        return connectionString;
    }
}
