using System.ComponentModel.DataAnnotations;

namespace DataFrontier.Application.DTOs.Auth;

/// <summary>
/// Credenciais para autenticação do usuário.
/// </summary>
public class LoginRequest
{
    /// <summary>
    /// E-mail do usuário (login).
    /// </summary>
    [Required(ErrorMessage = "E-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "E-mail inválido.")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Senha do usuário.
    /// </summary>
    [Required(ErrorMessage = "Senha é obrigatória.")]
    public string Senha { get; set; } = string.Empty;
}
