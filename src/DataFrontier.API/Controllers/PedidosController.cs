using DataFrontier.Application.DTOs.Pedidos;
using DataFrontier.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DataFrontier.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PedidosController : ControllerBase
{
    private readonly IPedidoService _pedidoService;
    private readonly IFiscalService _fiscalService;

    public PedidosController(IPedidoService pedidoService, IFiscalService fiscalService)
    {
        _pedidoService = pedidoService;
        _fiscalService = fiscalService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] string? status = null)
    {
        var result = await _pedidoService.GetAllAsync(pageNumber, pageSize, search, status);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _pedidoService.GetByIdAsync(id);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PedidoRequest request)
    {
        var result = await _pedidoService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] StatusPedidoRequest request)
    {
        var result = await _pedidoService.UpdateStatusAsync(id, request);
        return Ok(result);
    }

    /// <summary>
    /// Fatura o pedido: emite NF-e via ACBrLib, assina, transmite à SEFAZ,
    /// gera o PDF do DANFE e atualiza o status para 'Faturado'.
    /// </summary>
    [HttpPost("{id:guid}/faturar")]
    public async Task<IActionResult> Faturar(Guid id)
    {
        var result = await _fiscalService.EmitirNFeAsync(id);

        if (!result.Sucesso)
            return BadRequest(result);

        return Ok(result);
    }
}
