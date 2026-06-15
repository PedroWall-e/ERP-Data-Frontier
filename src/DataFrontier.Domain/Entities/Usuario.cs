namespace DataFrontier.Domain.Entities;

/// <summary>
/// Representa um usuário do sistema vinculado a um tenant.
/// Herda de <see cref="BaseEntity"/> para isolamento automático por tenant.
/// 
/// Cada usuário pertence a exatamente um tenant. A autenticação
/// é feita via e-mail + senha (hash BCrypt).
/// </summary>
public class Usuario : BaseEntity
{
    /// <summary>
    /// Endereço de e-mail do usuário. Utilizado como login.
    /// Deve ser único dentro do mesmo tenant.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Nome completo do usuário.
    /// </summary>
    public string NomeCompleto { get; set; } = string.Empty;

    /// <summary>
    /// Hash BCrypt da senha do usuário.
    /// Nunca armazene a senha em texto puro.
    /// </summary>
    public string SenhaHash { get; set; } = string.Empty;

    /// <summary>
    /// Indica se o usuário está ativo e pode fazer login.
    /// </summary>
    public bool Ativo { get; set; } = true;

    /// <summary>
    /// Data e hora UTC do último login bem-sucedido.
    /// </summary>
    public DateTime? UltimoLogin { get; set; }
}
