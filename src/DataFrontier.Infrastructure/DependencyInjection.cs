using DataFrontier.Application.Interfaces;
using DataFrontier.Domain.Interfaces;
using DataFrontier.Infrastructure.Fiscal;
using DataFrontier.Infrastructure.Persistence;
using DataFrontier.Infrastructure.Persistence.Interceptors;
using DataFrontier.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DataFrontier.Infrastructure;

/// <summary>
/// Métodos de extensão para registro dos serviços da camada de infraestrutura
/// no container de injeção de dependência do ASP.NET Core.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registra todos os serviços da camada de infraestrutura:
    /// - Multi-tenancy (ITenantResolver, ITenantConnectionResolver)
    /// - Interceptors (TenantInterceptor, AuditInterceptor)
    /// - Entity Framework Core (AppDbContext com connection string dinâmica)
    /// </summary>
    /// <param name="services">Coleção de serviços do ASP.NET Core.</param>
    /// <param name="configuration">Configuração da aplicação (appsettings).</param>
    /// <returns>A coleção de serviços para encadeamento.</returns>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ── Multi-Tenancy ────────────────────────────────────────────
        services.AddHttpContextAccessor();
        services.AddScoped<ITenantResolver, TenantResolver>();
        services.AddScoped<ITenantConnectionResolver, TenantConnectionResolver>();

        // ── Application Services ─────────────────────────────────────
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IProdutoService, ProdutoService>();
        services.AddScoped<IPessoaService, PessoaService>();
        services.AddScoped<IPedidoService, PedidoService>();

        // ── Fiscal (ACBrLib.NFe) ──────────────────────────────────────
        services.Configure<ACBrNFeConfig>(configuration.GetSection("ACBrNFe"));
        services.AddScoped<IFiscalService, ACBrFiscalService>();

        // ── Interceptors ─────────────────────────────────────────────
        services.AddScoped<TenantInterceptor>();
        services.AddScoped<AuditInterceptor>();

        // ── Entity Framework Core ────────────────────────────────────
        // O DbContext é registrado como Scoped.
        // A connection string é resolvida dinamicamente a cada requisição
        // pelo ITenantConnectionResolver, baseado no tier do tenant.
        //
        // IMPORTANTE: Não usar AddDbContextPool com connection strings dinâmicas.
        // O pool é inicializado com uma única connection string e não suporta
        // troca dinâmica entre requisições.
        services.AddDbContext<AppDbContext>((serviceProvider, options) =>
        {
            var tenantResolver = serviceProvider.GetRequiredService<ITenantResolver>();
            var connectionResolver = serviceProvider.GetRequiredService<ITenantConnectionResolver>();

            // Resolve a connection string baseada no tenant da requisição
            string connectionString;
            if (tenantResolver.IsResolved)
            {
                connectionString = connectionResolver.ResolveConnectionString(
                    tenantResolver.GetTenantId(),
                    tenantResolver.GetTenantTier());
            }
            else
            {
                // Fallback para requisições sem contexto de tenant
                // (health checks, migrations, etc.)
                connectionString = configuration.GetConnectionString("SharedDatabase")
                    ?? throw new InvalidOperationException(
                        "Connection string 'SharedDatabase' não configurada.");
            }

            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.MigrationsAssembly(
                    typeof(AppDbContext).Assembly.FullName);

                // Resiliência: retry automático em falhas transitórias
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorCodesToAdd: null);
            });

            // Registra os interceptors via DI
            var tenantInterceptor = serviceProvider.GetRequiredService<TenantInterceptor>();
            var auditInterceptor = serviceProvider.GetRequiredService<AuditInterceptor>();
            options.AddInterceptors(tenantInterceptor, auditInterceptor);

#if DEBUG
            options.EnableDetailedErrors();
            options.EnableSensitiveDataLogging();
#endif
        });

        return services;
    }
}
