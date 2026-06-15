using System.ComponentModel.DataAnnotations;

namespace DataFrontier.Application.DTOs.Auth;

/// <summary>
/// Dados necessários para registrar um novo tenant e seu usuário administrador.
/// </summary>
public class RegisterRequest
{
    /// <summary>
    /// E-mail do usuário administrador. Será usado como login.
    /// </summary>
    [Required(ErrorMessage = "E-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "E-mail inválido.")]
    [MaxLength(256)]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Senha do usuário. Mínimo 8 caracteres.
    /// </summary>
    [Required(ErrorMessage = "Senha é obrigatória.")]
    [MinLength(8, ErrorMessage = "A senha deve ter no mínimo 8 caracteres.")]
    public string Senha { get; set; } = string.Empty;

    /// <summary>
    /// Nome completo do usuário administrador.
    /// </summary>
    [Required(ErrorMessage = "Nome completo é obrigatório.")]
    [MaxLength(300)]
    public string NomeCompleto { get; set; } = string.Empty;

    /// <summary>
    /// Nome fantasia ou razão social da empresa (tenant).
    /// </summary>
    [Required(ErrorMessage = "Nome do tenant é obrigatório.")]
    [MaxLength(300)]
    public string NomeTenant { get; set; } = string.Empty;

    /// <summary>
    /// CNPJ da empresa. Formato: apenas dígitos (14 caracteres).
    /// </summary>
    [Required(ErrorMessage = "CNPJ é obrigatório.")]
    [StringLength(14, MinimumLength = 14, ErrorMessage = "CNPJ deve ter exatamente 14 dígitos.")]
    [RegularExpression(@"^\d{14}$", ErrorMessage = "CNPJ deve conter apenas dígitos.")]
    public string Cnpj { get; set; } = string.Empty;
}
