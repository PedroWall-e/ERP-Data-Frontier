using DataFrontier.Application.Interfaces;
using DataFrontier.Domain.Enums;
using DataFrontier.Domain.Exceptions;

namespace DataFrontier.API.Middleware;

/// <summary>
/// Middleware do ASP.NET Core responsável pela resolução do contexto de tenant.
/// 
/// Posição no pipeline: APÓS UseAuthentication() e ANTES de UseAuthorization().
/// Isso garante que as claims do JWT já foram decodificadas quando este middleware executa.
/// 
/// Fluxo:
/// 1. Verifica se a requisição é autenticada
/// 2. Extrai tenant_id e tenant_tier das claims do JWT
/// 3. Resolve a connection string via ITenantConnectionResolver
/// 4. Armazena a connection string em HttpContext.Items para uso pelo DbContext
/// 5. Requisições anônimas (health, swagger) passam sem validação de tenant
/// </summary>
public class TenantMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TenantMiddleware> _logger;

    /// <summary>
    /// Chave utilizada para armazenar a connection string resolvida no HttpContext.Items.
    /// </summary>
    public const string ConnectionStringKey = "TenantConnectionString";

    /// <summary>
    /// Chave utilizada para armazenar o TenantId resolvido no HttpContext.Items.
    /// </summary>
    public const string TenantIdKey = "TenantId";

    public TenantMiddleware(RequestDelegate next, ILogger<TenantMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// Intercepta a requisição HTTP para resolver o contexto de tenant.
    /// </summary>
    public async Task InvokeAsync(
        HttpContext context,
        ITenantConnectionResolver connectionResolver)
    {
        // Requisições não autenticadas passam direto (health checks, swagger, login)
        if (context.User.Identity?.IsAuthenticated != true)
        {
            await _next(context);
            return;
        }

        try
        {
            // Extrai as claims do JWT
            var tenantIdClaim = context.User.FindFirst("tenant_id")?.Value;
            var tenantTierClaim = context.User.FindFirst("tenant_tier")?.Value;

            if (string.IsNullOrWhiteSpace(tenantIdClaim))
            {
                _logger.LogWarning(
                    "Requisição autenticada sem claim 'tenant_id'. User: {UserId}",
                    context.User.FindFirst("sub")?.Value ?? "unknown");

                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsJsonAsync(new
                {
                    error = "Contexto de tenant não disponível.",
                    detail = "O token JWT não contém a claim 'tenant_id'."
                });
                return;
            }

            if (!Guid.TryParse(tenantIdClaim, out var tenantId))
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsJsonAsync(new
                {
                    error = "Claim 'tenant_id' inválida.",
                    detail = $"O valor '{tenantIdClaim}' não é um GUID válido."
                });
                return;
            }

            if (!Enum.TryParse<TenantTier>(tenantTierClaim, ignoreCase: true, out var tier))
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsJsonAsync(new
                {
                    error = "Claim 'tenant_tier' inválida.",
                    detail = $"Valor '{tenantTierClaim}' não reconhecido. " +
                             $"Aceitos: {string.Join(", ", Enum.GetNames<TenantTier>())}."
                });
                return;
            }

            // Resolve a connection string para o tenant
            var connectionString = connectionResolver.ResolveConnectionString(tenantId, tier);

            // Armazena no HttpContext.Items para uso downstream (DbContext, services)
            context.Items[TenantIdKey] = tenantId;
            context.Items[ConnectionStringKey] = connectionString;

            _logger.LogDebug(
                "Tenant resolvido: {TenantId}, Tier: {TenantTier}",
                tenantId, tier);
        }
        catch (TenantResolutionException ex)
        {
            _logger.LogError(ex, "Falha na resolução do tenant");
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsJsonAsync(new
            {
                error = "Falha na resolução do tenant.",
                detail = ex.Message
            });
            return;
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Erro ao resolver connection string do tenant");
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(new
            {
                error = "Erro interno na configuração do tenant.",
                detail = "Não foi possível resolver o banco de dados para este tenant."
            });
            return;
        }

        await _next(context);
    }
}

/// <summary>
/// Métodos de extensão para registrar o TenantMiddleware no pipeline do ASP.NET Core.
/// </summary>
public static class TenantMiddlewareExtensions
{
    /// <summary>
    /// Adiciona o middleware de resolução de tenant ao pipeline HTTP.
    /// Deve ser chamado APÓS UseAuthentication() e ANTES de UseAuthorization().
    /// </summary>
    public static IApplicationBuilder UseTenantResolution(this IApplicationBuilder app)
    {
        return app.UseMiddleware<TenantMiddleware>();
    }
}
