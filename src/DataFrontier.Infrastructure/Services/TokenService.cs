using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DataFrontier.Application.Interfaces;
using DataFrontier.Domain.Entities;
using DataFrontier.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DataFrontier.Infrastructure.Services;

/// <summary>
/// Gera tokens JWT com claims de tenant para autenticação no sistema multi-tenant.
/// 
/// Claims incluídas no token:
/// - sub: ID do usuário
/// - email: e-mail do usuário
/// - name: nome completo do usuário
/// - tenant_id: GUID do tenant
/// - tenant_tier: modalidade (SaaS/Enterprise)
/// </summary>
public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <inheritdoc/>
    public string GenerateToken(Usuario usuario, Tenant tenant)
    {
        var secretKey = _configuration["Jwt:SecretKey"]
            ?? throw new InvalidOperationException("Jwt:SecretKey não configurada.");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, usuario.Email),
            new Claim(JwtRegisteredClaimNames.Name, usuario.NomeCompleto),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("tenant_id", tenant.Id.ToString()),
            new Claim("tenant_tier", tenant.Tier.ToString())
        };

        var expiration = GetExpiration();

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: expiration,
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <inheritdoc/>
    public DateTime GetExpiration()
    {
        var hours = _configuration.GetValue<int>("Jwt:ExpirationHours", 8);
        return DateTime.UtcNow.AddHours(hours);
    }
}
