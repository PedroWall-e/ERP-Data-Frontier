namespace DataFrontier.Domain.Exceptions;

/// <summary>
/// Exceção lançada quando não é possível resolver o contexto de tenant
/// a partir da requisição HTTP atual.
/// 
/// Cenários comuns:
/// - Claim 'tenant_id' ausente no JWT
/// - Claim 'tenant_id' com formato inválido (não é um GUID válido)
/// - Claim 'tenant_tier' com valor não reconhecido
/// </summary>
public class TenantResolutionException : Exception
{
    /// <summary>
    /// Inicializa uma nova instância com a mensagem padrão.
    /// </summary>
    public TenantResolutionException()
        : base("Não foi possível resolver o contexto de tenant para esta requisição.")
    {
    }

    /// <summary>
    /// Inicializa uma nova instância com uma mensagem personalizada.
    /// </summary>
    /// <param name="message">Mensagem descritiva do erro.</param>
    public TenantResolutionException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Inicializa uma nova instância com mensagem e exceção interna.
    /// </summary>
    /// <param name="message">Mensagem descritiva do erro.</param>
    /// <param name="innerException">Exceção que originou este erro.</param>
    public TenantResolutionException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
