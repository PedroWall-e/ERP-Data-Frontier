namespace DataFrontier.Application.DTOs.Common;

/// <summary>
/// Resultado paginado genérico para consultas de listagem.
/// </summary>
/// <typeparam name="T">Tipo dos itens na lista.</typeparam>
public class PagedResult<T>
{
    /// <summary>
    /// Itens da página atual.
    /// </summary>
    public IReadOnlyList<T> Items { get; set; } = Array.Empty<T>();

    /// <summary>
    /// Total de registros no banco (sem paginação).
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Número da página atual (1-indexed).
    /// </summary>
    public int PageNumber { get; set; }

    /// <summary>
    /// Quantidade de itens por página.
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Total de páginas disponíveis.
    /// </summary>
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling(TotalCount / (double)PageSize) : 0;

    /// <summary>
    /// Indica se existe uma página anterior.
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;

    /// <summary>
    /// Indica se existe uma próxima página.
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;
}
