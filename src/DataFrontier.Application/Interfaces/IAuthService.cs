using DataFrontier.Application.DTOs.Auth;

namespace DataFrontier.Application.Interfaces;

/// <summary>
/// Contrato para o serviço de autenticação e registro de usuários.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Registra um novo tenant e seu usuário administrador.
    /// Criação atômica: se qualquer etapa falhar, toda a operação é revertida.
    /// </summary>
    /// <param name="request">Dados de registro.</param>
    /// <returns>Informações do tenant e usuário criados.</returns>
    Task<RegisterResponse> RegisterAsync(RegisterRequest request);

    /// <summary>
    /// Autentica um usuário com e-mail e senha, retornando um token JWT.
    /// </summary>
    /// <param name="request">Credenciais de login.</param>
    /// <returns>Token JWT e informações do usuário.</returns>
    Task<LoginResponse> LoginAsync(LoginRequest request);
}
