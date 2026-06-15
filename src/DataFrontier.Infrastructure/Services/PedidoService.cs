using DataFrontier.Application.DTOs.Common;
using DataFrontier.Application.DTOs.Pedidos;
using DataFrontier.Application.Interfaces;
using DataFrontier.Domain.Entities;
using DataFrontier.Domain.Enums;
using DataFrontier.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DataFrontier.Infrastructure.Services;

public class PedidoService : IPedidoService
{
    private readonly AppDbContext _context;

    public PedidoService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<PedidoResponse>> GetAllAsync(
        int pageNumber = 1, int pageSize = 10, string? search = null, string? status = null)
    {
        var query = _context.Pedidos
            .Include(p => p.Pessoa)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.ToLower();
            query = query.Where(p =>
                p.NumeroPedido.ToLower().Contains(term) ||
                p.Pessoa.RazaoSocial.ToLower().Contains(term) ||
                p.Pessoa.CpfCnpj.Contains(term));
        }

        if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<StatusPedido>(status, true, out var st))
            query = query.Where(p => p.Status == st);

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(p => p.DataEmissao)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<PedidoResponse>
        {
            Items = items.Select(MapToResponse).ToList(),
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize,
        };
    }

    public async Task<PedidoResponse> GetByIdAsync(Guid id)
    {
        var pedido = await _context.Pedidos
            .Include(p => p.Pessoa)
            .Include(p => p.Itens).ThenInclude(i => i.Impostos)
            .FirstOrDefaultAsync(p => p.Id == id)
            ?? throw new KeyNotFoundException($"Pedido com ID '{id}' não encontrado.");

        return MapToResponse(pedido);
    }

    public async Task<PedidoResponse> CreateAsync(PedidoRequest request)
    {
        var pessoa = await _context.Pessoas.FirstOrDefaultAsync(p => p.Id == request.PessoaId)
            ?? throw new KeyNotFoundException("Cliente não encontrado.");

        var count = await _context.Pedidos.CountAsync();
        var pedido = new Pedido
        {
            NumeroPedido = $"PED-{count + 1:D6}",
            DataEmissao = DateTime.UtcNow,
            PessoaId = request.PessoaId,
            Status = StatusPedido.Rascunho,
            Observacoes = request.Observacoes,
        };

        decimal totalProdutos = 0, totalDesconto = 0, totalImpostos = 0;

        foreach (var itemReq in request.Itens)
        {
            var produto = await _context.Produtos.FirstOrDefaultAsync(p => p.Id == itemReq.ProdutoId)
                ?? throw new KeyNotFoundException($"Produto '{itemReq.ProdutoId}' não encontrado.");

            var valorUnit = itemReq.ValorUnitario > 0 ? itemReq.ValorUnitario : produto.PrecoUnitario;
            var valorBruto = valorUnit * itemReq.Quantidade;
            var valorDesc = itemReq.ValorDesconto;
            var valorTotal = valorBruto - valorDesc;

            var item = new PedidoItem
            {
                ProdutoId = produto.Id,
                ProdutoNome = produto.Nome,
                ProdutoCodigo = produto.Codigo,
                Quantidade = itemReq.Quantidade,
                ValorUnitario = valorUnit,
                ValorDesconto = valorDesc,
                ValorTotal = valorTotal,
            };

            if (itemReq.Impostos is not null)
            {
                foreach (var impReq in itemReq.Impostos)
                {
                    item.Impostos.Add(new PedidoItemImposto
                    {
                        NomeImposto = impReq.NomeImposto,
                        BaseCalculo = impReq.BaseCalculo,
                        Aliquota = impReq.Aliquota,
                        ValorImposto = impReq.ValorImposto,
                    });
                    totalImpostos += impReq.ValorImposto;
                }
            }

            pedido.Itens.Add(item);
            totalProdutos += valorBruto;
            totalDesconto += valorDesc;
        }

        pedido.ValorTotalProdutos = totalProdutos;
        pedido.ValorTotalDesconto = totalDesconto;
        pedido.ValorTotalImpostos = totalImpostos;
        pedido.ValorTotalPedido = totalProdutos - totalDesconto;

        _context.Pedidos.Add(pedido);
        await _context.SaveChangesAsync();

        // Reload with navigation
        var saved = await _context.Pedidos
            .Include(p => p.Pessoa)
            .Include(p => p.Itens).ThenInclude(i => i.Impostos)
            .FirstAsync(p => p.Id == pedido.Id);

        return MapToResponse(saved);
    }

    public async Task<PedidoResponse> UpdateStatusAsync(Guid id, StatusPedidoRequest request)
    {
        var pedido = await _context.Pedidos
            .Include(p => p.Pessoa)
            .FirstOrDefaultAsync(p => p.Id == id)
            ?? throw new KeyNotFoundException($"Pedido com ID '{id}' não encontrado.");

        if (!Enum.TryParse<StatusPedido>(request.Status, true, out var newStatus))
            throw new InvalidOperationException($"Status '{request.Status}' inválido.");

        pedido.Status = newStatus;
        await _context.SaveChangesAsync();

        return MapToResponse(pedido);
    }

    private static PedidoResponse MapToResponse(Pedido p) => new()
    {
        Id = p.Id,
        NumeroPedido = p.NumeroPedido,
        DataEmissao = p.DataEmissao,
        Status = p.Status.ToString(),
        PessoaId = p.PessoaId,
        PessoaNome = p.Pessoa?.RazaoSocial ?? string.Empty,
        PessoaCpfCnpj = p.Pessoa?.CpfCnpj ?? string.Empty,
        ValorTotalProdutos = p.ValorTotalProdutos,
        ValorTotalDesconto = p.ValorTotalDesconto,
        ValorTotalImpostos = p.ValorTotalImpostos,
        ValorTotalPedido = p.ValorTotalPedido,
        Observacoes = p.Observacoes,
        NumeroNfe = p.NumeroNfe,
        ChaveAcessoNfe = p.ChaveAcessoNfe,
        CaminhoPdfDanfe = p.CaminhoPdfDanfe,
        NumeroNfse = p.NumeroNfse,
        CodigoVerificacaoNfse = p.CodigoVerificacaoNfse,
        LinkNfseNacional = p.LinkNfseNacional,
        CriadoEm = p.CriadoEm,
        Itens = p.Itens?.Select(i => new PedidoItemResponse
        {
            Id = i.Id,
            ProdutoId = i.ProdutoId,
            ProdutoNome = i.ProdutoNome,
            ProdutoCodigo = i.ProdutoCodigo,
            Quantidade = i.Quantidade,
            ValorUnitario = i.ValorUnitario,
            ValorDesconto = i.ValorDesconto,
            ValorTotal = i.ValorTotal,
            Impostos = i.Impostos?.Select(imp => new PedidoItemImpostoResponse
            {
                Id = imp.Id,
                NomeImposto = imp.NomeImposto,
                BaseCalculo = imp.BaseCalculo,
                Aliquota = imp.Aliquota,
                ValorImposto = imp.ValorImposto,
            }).ToList() ?? new(),
        }).ToList() ?? new(),
    };
}
