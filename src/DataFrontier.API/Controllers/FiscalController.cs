using DataFrontier.Application.DTOs.Fiscal;
using DataFrontier.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DataFrontier.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FiscalController : ControllerBase
{
    private readonly IFiscalService _fiscalService;

    public FiscalController(IFiscalService fiscalService)
    {
        _fiscalService = fiscalService;
    }

    /// <summary>Inutilizar faixa de numeração NF-e.</summary>
    [HttpPost("inutilizar")]
    public async Task<IActionResult> Inutilizar([FromBody] InutilizacaoRequest request)
    {
        var result = await _fiscalService.InutilizarNumeracaoAsync(
            request.Serie, request.NumeroInicio, request.NumeroFim, request.Justificativa);
        return result.Sucesso ? Ok(result) : BadRequest(result);
    }
}
