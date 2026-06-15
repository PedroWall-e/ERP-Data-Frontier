using DataFrontier.Application.DTOs.Auth;
using DataFrontier.Application.Interfaces;
using DataFrontier.Domain.Entities;
using DataFrontier.Domain.Enums;
using DataFrontier.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DataFrontier.Infrastructure.Services;

/// <summary>
/// Serviço de autenticação: registro de tenants e login de usuários.
/// 
/// O registro cria atomicamente um Tenant + Usuário administrador.
/// O login valida credenciais e gera um JWT com claims de tenant.
/// 
/// Nota: O AuthService acessa o DbContext com IgnoreQueryFilters
/// em operações de login/registro, pois o usuário ainda não está autenticado
/// e portanto não há contexto de tenant.
/// </summary>
public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly ITokenService _tokenService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        AppDbContext context,
        ITokenService tokenService,
        IConfiguration configuration,
        ILogger<AuthService> logger)
    {
        _context = context;
        _tokenService = tokenService;
        _configuration = configuration;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
    {
        // Verifica se o CNPJ já está cadastrado
        var cnpjExists = await _context.Tenants
            .AnyAsync(t => t.Cnpj == request.Cnpj);

        if (cnpjExists)
        {
            throw new InvalidOperationException(
                $"Já existe um tenant cadastrado com o CNPJ '{request.Cnpj}'.");
        }

        // Verifica se o e-mail já está em uso (globalmente)
        var emailExists = await _context.Usuarios
            .IgnoreQueryFilters()
            .AnyAsync(u => u.Email == request.Email);

        if (emailExists)
        {
            throw new InvalidOperationException(
                $"Já existe um usuário cadastrado com o e-mail '{request.Email}'.");
        }

        // Cria o tenant
        var tenant = new Tenant
        {
            Id = Guid.NewGuid(),
            Nome = request.NomeTenant,
            Cnpj = request.Cnpj,
            Tier = TenantTier.SaaS,
            Ativo = true,
            CriadoEm = DateTime.UtcNow
        };

        // Cria o usuário administrador
        var workFactor = _configuration.GetValue<int>("Auth:BcryptWorkFactor", 12);
        var usuario = new Usuario
        {
            Id = Guid.NewGuid(),
            TenantId = tenant.Id,
            Email = request.Email,
            NomeCompleto = request.NomeCompleto,
            SenhaHash = BCrypt.Net.BCrypt.HashPassword(request.Senha, workFactor),
            Ativo = true
        };

        // Persistência atômica (transação implícita do EF Core)
        _context.Tenants.Add(tenant);
        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Novo tenant registrado: {TenantId} ({TenantNome}) com usuário {Email}",
            tenant.Id, tenant.Nome, usuario.Email);

        return new RegisterResponse
        {
            UsuarioId = usuario.Id,
            TenantId = tenant.Id,
            Email = usuario.Email,
            NomeTenant = tenant.Nome
        };
    }

    /// <inheritdoc/>
    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        // Busca o usuário ignorando filtros de tenant (login é cross-tenant)
        var usuario = await _context.Usuarios
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Email == request.Email && u.Ativo);

        if (usuario is null)
        {
            _logger.LogWarning("Tentativa de login com e-mail não encontrado: {Email}", request.Email);
            throw new UnauthorizedAccessException("Credenciais inválidas.");
        }

        // Valida a senha
        if (!BCrypt.Net.BCrypt.Verify(request.Senha, usuario.SenhaHash))
        {
            _logger.LogWarning("Senha incorreta para usuário: {Email}", request.Email);
            throw new UnauthorizedAccessException("Credenciais inválidas.");
        }

        // Busca o tenant
        var tenant = await _context.Tenants
            .FirstOrDefaultAsync(t => t.Id == usuario.TenantId && t.Ativo);

        if (tenant is null)
        {
            _logger.LogError(
                "Tenant {TenantId} não encontrado ou inativo para usuário {Email}",
                usuario.TenantId, usuario.Email);
            throw new InvalidOperationException("Tenant não encontrado ou inativo.");
        }

        // Atualiza último login
        usuario.UltimoLogin = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        // Gera o token JWT
        var token = _tokenService.GenerateToken(usuario, tenant);
        var expiration = _tokenService.GetExpiration();

        _logger.LogInformation(
            "Login bem-sucedido: {Email} (Tenant: {TenantId})",
            usuario.Email, tenant.Id);

        return new LoginResponse
        {
            Token = token,
            ExpiresAt = expiration,
            TenantId = tenant.Id,
            TenantTier = tenant.Tier.ToString(),
            Email = usuario.Email,
            NomeCompleto = usuario.NomeCompleto
        };
    }
}
