namespace DataFrontier.Application.DTOs.Auth;

/// <summary>
/// Resposta de autenticação bem-sucedida contendo o token JWT.
/// </summary>
public class LoginResponse
{
    /// <summary>
    /// Token JWT para autenticação nas requisições subsequentes.
    /// Deve ser enviado no header Authorization: Bearer {token}.
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Data e hora UTC de expiração do token.
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// ID do tenant do usuário autenticado.
    /// </summary>
    public Guid TenantId { get; set; }

    /// <summary>
    /// Modalidade do tenant (SaaS ou Enterprise).
    /// </summary>
    public string TenantTier { get; set; } = string.Empty;

    /// <summary>
    /// E-mail do usuário autenticado.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Nome completo do usuário autenticado.
    /// </summary>
    public string NomeCompleto { get; set; } = string.Empty;
}
