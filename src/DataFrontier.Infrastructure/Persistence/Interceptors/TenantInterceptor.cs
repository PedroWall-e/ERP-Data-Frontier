using DataFrontier.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace DataFrontier.Infrastructure.Persistence.Interceptors;

/// <summary>
/// Interceptor do Entity Framework que popula automaticamente o TenantId
/// em todas as entidades <see cref="ITenantScoped"/> durante operações de INSERT.
/// 
/// Também valida que entidades modificadas não tiveram seu TenantId alterado,
/// prevenindo movimentação acidental de dados entre tenants.
/// 
/// Registrado como Scoped via DI e adicionado ao DbContext como interceptor.
/// </summary>
public class TenantInterceptor : SaveChangesInterceptor
{
    private readonly ITenantResolver _tenantResolver;

    public TenantInterceptor(ITenantResolver tenantResolver)
    {
        _tenantResolver = tenantResolver;
    }

    /// <inheritdoc/>
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is null || !_tenantResolver.IsResolved)
            return base.SavingChangesAsync(eventData, result, cancellationToken);

        var tenantId = _tenantResolver.GetTenantId();

        foreach (var entry in eventData.Context.ChangeTracker.Entries<ITenantScoped>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    // Auto-popula o TenantId em novas entidades
                    entry.Entity.TenantId = tenantId;
                    break;

                case EntityState.Modified:
                    // Impede alteração do TenantId em entidades existentes
                    if (entry.Entity.TenantId != tenantId)
                    {
                        throw new InvalidOperationException(
                            $"Tentativa de alterar o TenantId da entidade " +
                            $"{entry.Entity.GetType().Name}. " +
                            $"TenantId original: {entry.OriginalValues[nameof(ITenantScoped.TenantId)]}, " +
                            $"TenantId da requisição: {tenantId}. " +
                            $"Movimentação de dados entre tenants não é permitida.");
                    }
                    break;
            }
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    /// <inheritdoc/>
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        if (eventData.Context is null || !_tenantResolver.IsResolved)
            return base.SavingChanges(eventData, result);

        var tenantId = _tenantResolver.GetTenantId();

        foreach (var entry in eventData.Context.ChangeTracker.Entries<ITenantScoped>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.TenantId = tenantId;
                    break;

                case EntityState.Modified:
                    if (entry.Entity.TenantId != tenantId)
                    {
                        throw new InvalidOperationException(
                            $"Tentativa de alterar o TenantId da entidade " +
                            $"{entry.Entity.GetType().Name}. " +
                            $"Movimentação de dados entre tenants não é permitida.");
                    }
                    break;
            }
        }

        return base.SavingChanges(eventData, result);
    }
}
