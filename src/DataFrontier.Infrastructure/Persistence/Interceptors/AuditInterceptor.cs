using DataFrontier.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace DataFrontier.Infrastructure.Persistence.Interceptors;

/// <summary>
/// Interceptor do Entity Framework que popula automaticamente os campos
/// de auditoria (<see cref="BaseEntity.CriadoEm"/> e <see cref="BaseEntity.AtualizadoEm"/>)
/// em todas as entidades que herdam de <see cref="BaseEntity"/>.
/// 
/// - INSERT: Define <see cref="BaseEntity.CriadoEm"/> como DateTime.UtcNow.
/// - UPDATE: Define <see cref="BaseEntity.AtualizadoEm"/> como DateTime.UtcNow.
/// </summary>
public class AuditInterceptor : SaveChangesInterceptor
{
    /// <inheritdoc/>
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        ApplyAuditFields(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    /// <inheritdoc/>
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        ApplyAuditFields(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    private static void ApplyAuditFields(DbContext? context)
    {
        if (context is null)
            return;

        var utcNow = DateTime.UtcNow;

        foreach (var entry in context.ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CriadoEm = utcNow;
                    break;

                case EntityState.Modified:
                    entry.Entity.AtualizadoEm = utcNow;
                    // Impede alteração da data de criação em updates
                    entry.Property(nameof(BaseEntity.CriadoEm)).IsModified = false;
                    break;
            }
        }
    }
}
