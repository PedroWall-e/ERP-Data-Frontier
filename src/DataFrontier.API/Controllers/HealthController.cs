using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DataFrontier.API.Controllers;

/// <summary>
/// Controller para verificação de saúde da aplicação.
/// Endpoints públicos (sem autenticação) para uso em load balancers,
/// Kubernetes probes e monitoramento.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class HealthController : ControllerBase
{
    /// <summary>
    /// Verifica se a aplicação está respondendo.
    /// Retorna informações básicas sobre o ambiente.
    /// </summary>
    /// <returns>Status 200 OK com informações do ambiente.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Get()
    {
        return Ok(new
        {
            status = "healthy",
            service = "DataFrontier.API",
            version = typeof(HealthController).Assembly.GetName().Version?.ToString() ?? "1.0.0",
            timestamp = DateTime.UtcNow,
            environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown"
        });
    }
}
