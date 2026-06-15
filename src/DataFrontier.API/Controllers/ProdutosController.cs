using DataFrontier.Application.DTOs.Common;
using DataFrontier.Application.DTOs.Produtos;
using DataFrontier.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DataFrontier.API.Controllers;

/// <summary>
/// Controller de CRUD de produtos.
/// Todos os endpoints requerem autenticação JWT.
/// O isolamento por tenant é automático via Global Query Filters.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProdutosController : ControllerBase
{
    private readonly IProdutoService _produtoService;

    public ProdutosController(IProdutoService produtoService)
    {
        _produtoService = produtoService;
    }

    /// <summary>
    /// Lista produtos paginados do tenant autenticado.
    /// </summary>
    /// <param name="pageNumber">Página (default: 1).</param>
    /// <param name="pageSize">Itens por página (default: 20, máx: 100).</param>
    /// <param name="search">Filtro por nome ou código.</param>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<ProdutoResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null)
    {
        var result = await _produtoService.GetAllAsync(pageNumber, pageSize, search);
        return Ok(result);
    }

    /// <summary>
    /// Obtém um produto por ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProdutoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _produtoService.GetByIdAsync(id);

        if (result is null)
            return NotFound(new { error = "Produto não encontrado." });

        return Ok(result);
    }

    /// <summary>
    /// Cria um novo produto para o tenant autenticado.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ProdutoResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] ProdutoRequest request)
    {
        var result = await _produtoService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Atualiza um produto existente.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ProdutoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(Guid id, [FromBody] ProdutoRequest request)
    {
        var result = await _produtoService.UpdateAsync(id, request);

        if (result is null)
            return NotFound(new { error = "Produto não encontrado." });

        return Ok(result);
    }

    /// <summary>
    /// Desativa um produto (soft delete).
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _produtoService.DeleteAsync(id);

        if (!deleted)
            return NotFound(new { error = "Produto não encontrado." });

        return NoContent();
    }
}
