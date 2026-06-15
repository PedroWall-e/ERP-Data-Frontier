using DataFrontier.Application.DTOs.Auth;
using DataFrontier.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DataFrontier.API.Controllers;

/// <summary>
/// Controller de autenticação: registro de tenants e login de usuários.
/// Todos os endpoints são públicos (AllowAnonymous), exceto /me.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Registra um novo tenant e seu usuário administrador.
    /// Criação atômica: tenant + usuário na mesma transação.
    /// </summary>
    /// <param name="request">Dados de registro (e-mail, senha, nome, CNPJ).</param>
    /// <returns>Informações do tenant e usuário criados.</returns>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(RegisterResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(request);
        return StatusCode(StatusCodes.Status201Created, result);
    }

    /// <summary>
    /// Autentica um usuário com e-mail e senha.
    /// Retorna um token JWT com claims de tenant para uso nas requisições subsequentes.
    /// </summary>
    /// <param name="request">Credenciais (e-mail e senha).</param>
    /// <returns>Token JWT, data de expiração e dados do tenant.</returns>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);
        return Ok(result);
    }

    /// <summary>
    /// Retorna os dados do usuário autenticado e seu tenant.
    /// Requer autenticação via JWT Bearer.
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult Me()
    {
        var userId = User.FindFirst("sub")?.Value;
        var email = User.FindFirst("email")?.Value;
        var name = User.FindFirst("name")?.Value;
        var tenantId = User.FindFirst("tenant_id")?.Value;
        var tenantTier = User.FindFirst("tenant_tier")?.Value;

        return Ok(new
        {
            userId,
            email,
            name,
            tenantId,
            tenantTier
        });
    }
}
