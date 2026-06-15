using DataFrontier.Application.DTOs.Common;
using DataFrontier.Application.DTOs.Pessoas;

namespace DataFrontier.Application.Interfaces;

public interface IPessoaService
{
    Task<PagedResult<PessoaResponse>> GetAllAsync(int pageNumber = 1, int pageSize = 10, string? search = null, string? tipo = null);
    Task<PessoaResponse> GetByIdAsync(Guid id);
    Task<PessoaResponse> CreateAsync(PessoaRequest request);
    Task<PessoaResponse> UpdateAsync(Guid id, PessoaRequest request);
    Task DeleteAsync(Guid id);
}
