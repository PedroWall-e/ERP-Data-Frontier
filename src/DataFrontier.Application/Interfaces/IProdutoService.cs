using DataFrontier.Application.DTOs.Common;
using DataFrontier.Application.DTOs.Produtos;

namespace DataFrontier.Application.Interfaces;

/// <summary>
/// Contrato para operações CRUD de produtos.
/// O isolamento por tenant é garantido automaticamente pelos Global Query Filters.
/// </summary>
public interface IProdutoService
{
    /// <summary>
    /// Cria um novo produto para o tenant da requisição atual.
    /// </summary>
    Task<ProdutoResponse> CreateAsync(ProdutoRequest request);

    /// <summary>
    /// Obtém um produto por ID (filtrado pelo tenant automaticamente).
    /// </summary>
    Task<ProdutoResponse?> GetByIdAsync(Guid id);

    /// <summary>
    /// Lista produtos paginados do tenant da requisição atual.
    /// </summary>
    /// <param name="pageNumber">Número da página (1-indexed, default: 1).</param>
    /// <param name="pageSize">Itens por página (default: 20, máx: 100).</param>
    /// <param name="search">Filtro opcional por nome ou código.</param>
    Task<PagedResult<ProdutoResponse>> GetAllAsync(int pageNumber = 1, int pageSize = 20, string? search = null);

    /// <summary>
    /// Atualiza um produto existente.
    /// </summary>
    Task<ProdutoResponse?> UpdateAsync(Guid id, ProdutoRequest request);

    /// <summary>
    /// Desativa um produto (soft delete: Ativo = false).
    /// </summary>
    Task<bool> DeleteAsync(Guid id);
}
