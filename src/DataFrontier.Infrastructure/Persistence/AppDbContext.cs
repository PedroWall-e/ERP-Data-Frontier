using System.Linq.Expressions;
using DataFrontier.Domain.Entities;
using DataFrontier.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DataFrontier.Infrastructure.Persistence;

/// <summary>
/// Contexto principal do Entity Framework Core com suporte a multi-tenancy híbrida.
/// 
/// Funcionalidades:
/// - Global Query Filters dinâmicos: aplicados automaticamente a todas as entidades
///   que implementam <see cref="ITenantScoped"/>, garantindo isolamento por tenant.
/// - O filtro referencia a propriedade <see cref="CurrentTenantId"/> que é avaliada
///   a cada execução de query (não no momento da criação do modelo).
/// - O SQL gerado é parametrizado (WHERE "TenantId" = @__CurrentTenantId),
///   permitindo cache do plano de execução pelo PostgreSQL.
/// 
/// Para cenários Enterprise (banco dedicado), os filtros atuam como safety net
/// adicional, prevenindo vazamento de dados em caso de erro de configuração.
/// </summary>
public class AppDbContext : DbContext
{
    private readonly ITenantResolver _tenantResolver;

    /// <summary>
    /// Inicializa o DbContext com as opções de configuração e o resolver de tenant.
    /// </summary>
    /// <param name="options">Opções de configuração do DbContext (connection string, provider, etc.).</param>
    /// <param name="tenantResolver">Serviço de resolução do tenant da requisição atual.</param>
    public AppDbContext(DbContextOptions<AppDbContext> options, ITenantResolver tenantResolver)
        : base(options)
    {
        _tenantResolver = tenantResolver;
    }

    /// <summary>
    /// TenantId da requisição atual. Avaliado dinamicamente a cada query pelo EF Core.
    /// O EF Core gera SQL parametrizado: WHERE "TenantId" = @__CurrentTenantId.
    /// </summary>
    public Guid CurrentTenantId => _tenantResolver.IsResolved
        ? _tenantResolver.GetTenantId()
        : Guid.Empty;

    // ── DbSets ───────────────────────────────────────────────────────

    /// <summary>Tenants (organizações) cadastrados no sistema.</summary>
    public DbSet<Tenant> Tenants => Set<Tenant>();

    /// <summary>Usuários do sistema (isolados por tenant).</summary>
    public DbSet<Usuario> Usuarios => Set<Usuario>();

    /// <summary>Produtos e serviços cadastrados.</summary>
    public DbSet<Produto> Produtos => Set<Produto>();

    /// <summary>Impostos calculados por item de documento fiscal.</summary>
    public DbSet<ImpostoDoItem> ImpostosDoItem => Set<ImpostoDoItem>();

    /// <summary>Pessoas (Clientes e/ou Fornecedores).</summary>
    public DbSet<Pessoa> Pessoas => Set<Pessoa>();

    /// <summary>Endereços vinculados a Pessoas.</summary>
    public DbSet<Endereco> Enderecos => Set<Endereco>();

    /// <summary>Pedidos de venda.</summary>
    public DbSet<Pedido> Pedidos => Set<Pedido>();

    /// <summary>Itens de pedidos de venda.</summary>
    public DbSet<PedidoItem> PedidoItens => Set<PedidoItem>();

    /// <summary>Impostos por item de pedido (Reforma Tributária ready).</summary>
    public DbSet<PedidoItemImposto> PedidoItemImpostos => Set<PedidoItemImposto>();

    // ── Model Configuration ──────────────────────────────────────────

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Aplica todas as configurações de entidade do assembly atual
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // Aplica Global Query Filters dinamicamente para todas as entidades ITenantScoped
        ApplyTenantQueryFilters(modelBuilder);
    }

    /// <summary>
    /// Aplica Global Query Filters a todas as entidades que implementam <see cref="ITenantScoped"/>.
    /// 
    /// Para cada entidade, gera a seguinte expression tree:
    ///   e => EF.Property&lt;Guid&gt;(e, "TenantId") == this.CurrentTenantId
    /// 
    /// A referência a <c>this.CurrentTenantId</c> garante que o valor é avaliado
    /// a cada execução de query, não no momento da criação do modelo.
    /// </summary>
    private void ApplyTenantQueryFilters(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (!typeof(ITenantScoped).IsAssignableFrom(entityType.ClrType))
                continue;

            // Parâmetro: e (do tipo da entidade)
            var parameter = Expression.Parameter(entityType.ClrType, "e");

            // Lado esquerdo: e.TenantId
            var tenantIdProperty = Expression.Property(parameter, nameof(ITenantScoped.TenantId));

            // Lado direito: this.CurrentTenantId (referência ao DbContext, avaliada por query)
            var currentTenantId = Expression.Property(
                Expression.Constant(this),
                nameof(CurrentTenantId)
            );

            // Expressão completa: e.TenantId == this.CurrentTenantId
            var filter = Expression.Lambda(
                Expression.Equal(tenantIdProperty, currentTenantId),
                parameter
            );

            modelBuilder.Entity(entityType.ClrType).HasQueryFilter(filter);
        }
    }
}
