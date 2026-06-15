using DataFrontier.Application.DTOs.Empresa;
using DataFrontier.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DataFrontier.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EmpresaController : ControllerBase
{
    private readonly IEmpresaService _empresaService;

    public EmpresaController(IEmpresaService empresaService)
    {
        _empresaService = empresaService;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var result = await _empresaService.GetAsync();
        return Ok(result);
    }

    [HttpPut]
    public async Task<IActionResult> Save([FromBody] ConfiguracaoEmpresaRequest request)
    {
        var result = await _empresaService.SaveAsync(request);
        return Ok(result);
    }

    [HttpPost("certificado")]
    public async Task<IActionResult> UploadCertificado(
        [FromForm] IFormFile arquivo,
        [FromForm] string senha)
    {
        using var stream = arquivo.OpenReadStream();
        var result = await _empresaService.UploadCertificadoAsync(stream, arquivo.FileName, senha);
        return Ok(result);
    }

    [HttpDelete("certificado")]
    public async Task<IActionResult> RemoverCertificado()
    {
        await _empresaService.RemoverCertificadoAsync();
        return NoContent();
    }
}
