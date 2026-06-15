using DataFrontier.Application.DTOs.Common;
using DataFrontier.Application.DTOs.Pedidos;

namespace DataFrontier.Application.Interfaces;

public interface IPedidoService
{
    Task<PagedResult<PedidoResponse>> GetAllAsync(int pageNumber = 1, int pageSize = 10, string? search = null, string? status = null);
    Task<PedidoResponse> GetByIdAsync(Guid id);
    Task<PedidoResponse> CreateAsync(PedidoRequest request);
    Task<PedidoResponse> UpdateStatusAsync(Guid id, StatusPedidoRequest request);
}
