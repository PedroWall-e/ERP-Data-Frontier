using DataFrontier.Application.DTOs.Fiscal;
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
    private readonly INFSeService _nfseService;

    public PedidosController(IPedidoService pedidoService, IFiscalService fiscalService, INFSeService nfseService)
    {
        _pedidoService = pedidoService;
        _fiscalService = fiscalService;
        _nfseService = nfseService;
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

    /// <summary>
    /// Fatura um pedido de serviço: emite NFS-e via ACBrLib.NFSe Nacional.
    /// </summary>
    [HttpPost("{id:guid}/faturar-servico")]
    public async Task<IActionResult> FaturarServico(Guid id)
    {
        var result = await _nfseService.EmitirNFSeAsync(id);
        if (!result.Sucesso)
            return BadRequest(result);
        return Ok(result);
    }

    /// <summary>Cancelar NF-e emitida.</summary>
    [HttpPost("{id:guid}/cancelar-nfe")]
    public async Task<IActionResult> CancelarNFe(Guid id, [FromBody] CancelamentoRequest request)
    {
        var result = await _fiscalService.CancelarNFeAsync(id, request.Justificativa);
        return result.Sucesso ? Ok(result) : BadRequest(result);
    }

    /// <summary>Carta de Correção (CC-e).</summary>
    [HttpPost("{id:guid}/carta-correcao")]
    public async Task<IActionResult> CartaCorrecao(Guid id, [FromBody] CartaCorrecaoRequest request)
    {
        var result = await _fiscalService.CartaCorrecaoAsync(id, request.Texto);
        return result.Sucesso ? Ok(result) : BadRequest(result);
    }

    /// <summary>Consultar NF-e na SEFAZ.</summary>
    [HttpGet("{id:guid}/consultar-sefaz")]
    public async Task<IActionResult> ConsultarSefaz(Guid id)
    {
        var result = await _fiscalService.ConsultarNFeAsync(id);
        return result.Sucesso ? Ok(result) : BadRequest(result);
    }

    /// <summary>Download do XML da NF-e.</summary>
    [HttpGet("{id:guid}/xml")]
    public async Task<IActionResult> DownloadXml(Guid id, [FromServices] IXmlStorageService xmlStorage)
    {
        var pedido = await _pedidoService.GetByIdAsync(id);
        if (string.IsNullOrEmpty(pedido.ChaveAcessoNfe))
            return NotFound(new { error = "NF-e não encontrada." });

        var xml = await xmlStorage.ObterBytesAsync(Guid.Empty, pedido.ChaveAcessoNfe, "nfe");
        if (xml is null)
            return NotFound(new { error = "XML não encontrado no armazenamento." });

        return File(xml, "application/xml", $"{pedido.ChaveAcessoNfe}-nfe.xml");
    }

    /// <summary>
    /// Download do PDF do DANFE da NF-e emitida.
    /// </summary>
    [HttpGet("{id:guid}/danfe")]
    public async Task<IActionResult> DownloadDanfe(Guid id)
    {
        var pedido = await _pedidoService.GetByIdAsync(id);

        if (string.IsNullOrEmpty(pedido.CaminhoPdfDanfe))
            return NotFound(new { error = "DANFE não disponível. Pedido ainda não foi faturado." });

        if (!System.IO.File.Exists(pedido.CaminhoPdfDanfe))
            return NotFound(new { error = "Arquivo PDF do DANFE não encontrado no servidor." });

        var fileBytes = await System.IO.File.ReadAllBytesAsync(pedido.CaminhoPdfDanfe);
        return File(fileBytes, "application/pdf", $"DANFE-{pedido.NumeroPedido}.pdf");
    }
}
