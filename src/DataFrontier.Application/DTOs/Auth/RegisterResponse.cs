namespace DataFrontier.Application.DTOs.Auth;

/// <summary>
/// Resposta do registro de um novo tenant.
/// </summary>
public class RegisterResponse
{
    /// <summary>
    /// ID do usuário administrador criado.
    /// </summary>
    public Guid UsuarioId { get; set; }

    /// <summary>
    /// ID do tenant criado.
    /// </summary>
    public Guid TenantId { get; set; }

    /// <summary>
    /// E-mail do usuário registrado.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Nome do tenant criado.
    /// </summary>
    public string NomeTenant { get; set; } = string.Empty;
}
