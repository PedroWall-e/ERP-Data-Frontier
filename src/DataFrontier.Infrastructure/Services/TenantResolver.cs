using DataFrontier.Domain.Enums;
using DataFrontier.Domain.Exceptions;
using DataFrontier.Domain.Interfaces;
using Microsoft.AspNetCore.Http;

namespace DataFrontier.Infrastructure.Services;

/// <summary>
/// Implementação de <see cref="ITenantResolver"/> que extrai o contexto de tenant
/// a partir das claims do token JWT presente na requisição HTTP atual.
/// 
/// Claims esperadas no JWT:
/// - <c>tenant_id</c>: GUID do tenant (obrigatória para requisições autenticadas)
/// - <c>tenant_tier</c>: Modalidade "SaaS" ou "Enterprise" (obrigatória para requisições autenticadas)
/// 
/// Lifetime: Scoped (uma instância por requisição HTTP).
/// </summary>
public class TenantResolver : ITenantResolver
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private Guid? _cachedTenantId;
    private TenantTier? _cachedTier;

    /// <summary>
    /// Nome da claim que contém o identificador do tenant no JWT.
    /// </summary>
    public const string TenantIdClaimType = "tenant_id";

    /// <summary>
    /// Nome da claim que contém a modalidade do tenant no JWT.
    /// </summary>
    public const string TenantTierClaimType = "tenant_tier";

    public TenantResolver(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <inheritdoc/>
    public bool IsResolved
    {
        get
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext is null)
                return false;

            return httpContext.User.Identity?.IsAuthenticated == true
                && httpContext.User.FindFirst(TenantIdClaimType) is not null;
        }
    }

    /// <inheritdoc/>
    public Guid GetTenantId()
    {
        if (_cachedTenantId.HasValue)
            return _cachedTenantId.Value;

        var httpContext = _httpContextAccessor.HttpContext
            ?? throw new TenantResolutionException(
                "HttpContext não disponível. O ITenantResolver deve ser utilizado " +
                "apenas no contexto de uma requisição HTTP.");

        var tenantIdClaim = httpContext.User.FindFirst(TenantIdClaimType)?.Value;

        if (string.IsNullOrWhiteSpace(tenantIdClaim))
        {
            throw new TenantResolutionException(
                $"Claim '{TenantIdClaimType}' não encontrada no token JWT. " +
                "Verifique se o token contém as claims de tenant.");
        }

        if (!Guid.TryParse(tenantIdClaim, out var tenantId))
        {
            throw new TenantResolutionException(
                $"Claim '{TenantIdClaimType}' possui formato inválido: '{tenantIdClaim}'. " +
                "O valor deve ser um GUID válido.");
        }

        _cachedTenantId = tenantId;
        return tenantId;
    }

    /// <inheritdoc/>
    public TenantTier GetTenantTier()
    {
        if (_cachedTier.HasValue)
            return _cachedTier.Value;

        var httpContext = _httpContextAccessor.HttpContext
            ?? throw new TenantResolutionException(
                "HttpContext não disponível.");

        var tierClaim = httpContext.User.FindFirst(TenantTierClaimType)?.Value;

        if (string.IsNullOrWhiteSpace(tierClaim))
        {
            throw new TenantResolutionException(
                $"Claim '{TenantTierClaimType}' não encontrada no token JWT.");
        }

        if (!Enum.TryParse<TenantTier>(tierClaim, ignoreCase: true, out var tier))
        {
            throw new TenantResolutionException(
                $"Claim '{TenantTierClaimType}' possui valor não reconhecido: '{tierClaim}'. " +
                $"Valores aceitos: {string.Join(", ", Enum.GetNames<TenantTier>())}.");
        }

        _cachedTier = tier;
        return tier;
    }
}
