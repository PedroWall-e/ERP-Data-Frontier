using DataFrontier.Application.DTOs.Common;
using DataFrontier.Application.DTOs.Produtos;
using DataFrontier.Application.Interfaces;
using DataFrontier.Domain.Entities;
using DataFrontier.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DataFrontier.Infrastructure.Services;

/// <summary>
/// Implementação do serviço de CRUD de produtos.
/// 
/// O isolamento por tenant é garantido automaticamente pelos Global Query Filters
/// do AppDbContext. Todas as queries são filtradas por TenantId sem necessidade
/// de filtros explícitos.
/// 
/// O TenantId é populado automaticamente pelo TenantInterceptor nos INSERTs.
/// </summary>
public class ProdutoService : IProdutoService
{
    private readonly AppDbContext _context;

    public ProdutoService(AppDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc/>
    public async Task<ProdutoResponse> CreateAsync(ProdutoRequest request)
    {
        var produto = new Produto
        {
            Id = Guid.NewGuid(),
            Nome = request.Nome,
            Descricao = request.Descricao,
            Codigo = request.Codigo,
            CodigoNcm = request.CodigoNcm,
            PrecoUnitario = request.PrecoUnitario,
            Ativo = true
        };

        _context.Produtos.Add(produto);
        await _context.SaveChangesAsync();

        return MapToResponse(produto);
    }

    /// <inheritdoc/>
    public async Task<ProdutoResponse?> GetByIdAsync(Guid id)
    {
        var produto = await _context.Produtos
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

        return produto is null ? null : MapToResponse(produto);
    }

    /// <inheritdoc/>
    public async Task<PagedResult<ProdutoResponse>> GetAllAsync(
        int pageNumber = 1,
        int pageSize = 20,
        string? search = null)
    {
        pageNumber = Math.Max(1, pageNumber);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var query = _context.Produtos
            .AsNoTracking()
            .Where(p => p.Ativo);

        // Filtro de busca por nome ou código
        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchLower = search.ToLower();
            query = query.Where(p =>
                p.Nome.ToLower().Contains(searchLower) ||
                p.Codigo.ToLower().Contains(searchLower));
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderBy(p => p.Nome)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(p => MapToResponse(p))
            .ToListAsync();

        return new PagedResult<ProdutoResponse>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    /// <inheritdoc/>
    public async Task<ProdutoResponse?> UpdateAsync(Guid id, ProdutoRequest request)
    {
        var produto = await _context.Produtos
            .FirstOrDefaultAsync(p => p.Id == id);

        if (produto is null)
            return null;

        produto.Nome = request.Nome;
        produto.Descricao = request.Descricao;
        produto.Codigo = request.Codigo;
        produto.CodigoNcm = request.CodigoNcm;
        produto.PrecoUnitario = request.PrecoUnitario;

        await _context.SaveChangesAsync();

        return MapToResponse(produto);
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteAsync(Guid id)
    {
        var produto = await _context.Produtos
            .FirstOrDefaultAsync(p => p.Id == id);

        if (produto is null)
            return false;

        // Soft delete
        produto.Ativo = false;
        await _context.SaveChangesAsync();

        return true;
    }

    private static ProdutoResponse MapToResponse(Produto produto)
    {
        return new ProdutoResponse
        {
            Id = produto.Id,
            Nome = produto.Nome,
            Descricao = produto.Descricao,
            Codigo = produto.Codigo,
            CodigoNcm = produto.CodigoNcm,
            PrecoUnitario = produto.PrecoUnitario,
            Ativo = produto.Ativo,
            CriadoEm = produto.CriadoEm,
            AtualizadoEm = produto.AtualizadoEm
        };
    }
}
