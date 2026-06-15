using DataFrontier.Domain.Entities;
using DataFrontier.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DataFrontier.Infrastructure.Persistence.Seed;

/// <summary>
/// Seed de dados iniciais para ambiente de desenvolvimento.
/// Cria um tenant demo e usuário admin para testes imediatos via Swagger.
/// 
/// Credenciais de teste:
/// - E-mail: admin@demo.com
/// - Senha: Admin@123
/// - Tenant: Empresa Demo Ltda (CNPJ: 12345678000190)
/// </summary>
public static class DatabaseSeeder
{
    /// <summary>
    /// Aplica migrations pendentes e insere dados de seed se o banco estiver vazio.
    /// </summary>
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<AppDbContext>>();

        try
        {
            // Aplica migrations pendentes
            logger.LogInformation("Aplicando migrations pendentes...");
            await context.Database.MigrateAsync();
            logger.LogInformation("Migrations aplicadas com sucesso.");

            // Seed apenas se não existir nenhum tenant
            if (await context.Tenants.AnyAsync())
            {
                logger.LogInformation("Banco já possui dados. Seed ignorado.");
                return;
            }

            logger.LogInformation("Inserindo dados de seed...");

            var tenantId = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890");

            var tenant = new Tenant
            {
                Id = tenantId,
                Nome = "Empresa Demo Ltda",
                Cnpj = "12345678000190",
                Tier = TenantTier.SaaS,
                Ativo = true,
                CriadoEm = DateTime.UtcNow
            };

            var admin = new Usuario
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                Email = "admin@demo.com",
                NomeCompleto = "Administrador Demo",
                SenhaHash = BCrypt.Net.BCrypt.HashPassword("Admin@123", 12),
                Ativo = true,
                CriadoEm = DateTime.UtcNow
            };

            var produtos = new[]
            {
                new Produto
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    Nome = "Notebook Dell Inspiron 15",
                    Descricao = "Notebook Dell Inspiron 15, Intel Core i7, 16GB RAM, 512GB SSD",
                    Codigo = "NB-DELL-001",
                    CodigoNcm = "84713012",
                    PrecoUnitario = 4299.90m,
                    Ativo = true,
                    CriadoEm = DateTime.UtcNow
                },
                new Produto
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    Nome = "Mouse Logitech MX Master 3S",
                    Descricao = "Mouse sem fio ergonômico com sensor 8000 DPI",
                    Codigo = "MS-LOG-001",
                    CodigoNcm = "84716029",
                    PrecoUnitario = 549.90m,
                    Ativo = true,
                    CriadoEm = DateTime.UtcNow
                },
                new Produto
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    Nome = "Monitor LG UltraWide 34\"",
                    Descricao = "Monitor LG 34WP550-B, IPS, Full HD, 75Hz",
                    Codigo = "MN-LG-001",
                    CodigoNcm = "85285220",
                    PrecoUnitario = 1899.00m,
                    Ativo = true,
                    CriadoEm = DateTime.UtcNow
                }
            };

            context.Tenants.Add(tenant);
            context.Usuarios.Add(admin);
            context.Produtos.AddRange(produtos);

            await context.SaveChangesAsync();

            logger.LogInformation(
                "Seed concluído: Tenant '{TenantNome}', 1 usuário admin, {ProdutoCount} produtos",
                tenant.Nome, produtos.Length);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao executar seed do banco de dados");
            throw;
        }
    }
}
