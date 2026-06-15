using DataFrontier.Application.DTOs.Common;
using DataFrontier.Application.DTOs.Pessoas;
using DataFrontier.Application.Interfaces;
using DataFrontier.Domain.Entities;
using DataFrontier.Domain.Enums;
using DataFrontier.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DataFrontier.Infrastructure.Services;

public class PessoaService : IPessoaService
{
    private readonly AppDbContext _context;

    public PessoaService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<PessoaResponse>> GetAllAsync(
        int pageNumber = 1, int pageSize = 10, string? search = null, string? tipo = null)
    {
        var query = _context.Pessoas
            .Include(p => p.Endereco)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.ToLower();
            query = query.Where(p =>
                p.RazaoSocial.ToLower().Contains(term) ||
                p.CpfCnpj.Contains(term) ||
                (p.NomeFantasia != null && p.NomeFantasia.ToLower().Contains(term)) ||
                (p.Email != null && p.Email.ToLower().Contains(term)));
        }

        if (!string.IsNullOrWhiteSpace(tipo))
        {
            if (tipo.Equals("cliente", StringComparison.OrdinalIgnoreCase))
                query = query.Where(p => p.IsCliente);
            else if (tipo.Equals("fornecedor", StringComparison.OrdinalIgnoreCase))
                query = query.Where(p => p.IsFornecedor);
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderBy(p => p.RazaoSocial)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<PessoaResponse>
        {
            Items = items.Select(MapToResponse).ToList(),
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize,
        };
    }

    public async Task<PessoaResponse> GetByIdAsync(Guid id)
    {
        var pessoa = await _context.Pessoas
            .Include(p => p.Endereco)
            .FirstOrDefaultAsync(p => p.Id == id)
            ?? throw new KeyNotFoundException($"Pessoa com ID '{id}' não encontrada.");

        return MapToResponse(pessoa);
    }

    public async Task<PessoaResponse> CreateAsync(PessoaRequest request)
    {
        var exists = await _context.Pessoas.AnyAsync(p => p.CpfCnpj == request.CpfCnpj);
        if (exists)
            throw new InvalidOperationException(
                $"Já existe uma pessoa com o CPF/CNPJ '{request.CpfCnpj}' neste tenant.");

        var pessoa = new Pessoa
        {
            TipoPessoa = Enum.Parse<TipoPessoa>(request.TipoPessoa),
            CpfCnpj = request.CpfCnpj,
            RazaoSocial = request.RazaoSocial,
            NomeFantasia = request.NomeFantasia,
            InscricaoEstadual = request.InscricaoEstadual,
            IndicadorIE = Enum.Parse<IndicadorIE>(request.IndicadorIE),
            IsCliente = request.IsCliente,
            IsFornecedor = request.IsFornecedor,
            Email = request.Email,
            Telefone = request.Telefone,
        };

        if (request.Endereco is not null)
        {
            pessoa.Endereco = new Endereco
            {
                Logradouro = request.Endereco.Logradouro,
                Numero = request.Endereco.Numero,
                Complemento = request.Endereco.Complemento,
                Bairro = request.Endereco.Bairro,
                Cep = request.Endereco.Cep,
                Cidade = request.Endereco.Cidade,
                Uf = request.Endereco.Uf,
                CodigoIbge = request.Endereco.CodigoIbge,
            };
        }

        _context.Pessoas.Add(pessoa);
        await _context.SaveChangesAsync();

        return MapToResponse(pessoa);
    }

    public async Task<PessoaResponse> UpdateAsync(Guid id, PessoaRequest request)
    {
        var pessoa = await _context.Pessoas
            .Include(p => p.Endereco)
            .FirstOrDefaultAsync(p => p.Id == id)
            ?? throw new KeyNotFoundException($"Pessoa com ID '{id}' não encontrada.");

        var duplicate = await _context.Pessoas
            .AnyAsync(p => p.CpfCnpj == request.CpfCnpj && p.Id != id);
        if (duplicate)
            throw new InvalidOperationException(
                $"Já existe outra pessoa com o CPF/CNPJ '{request.CpfCnpj}' neste tenant.");

        pessoa.TipoPessoa = Enum.Parse<TipoPessoa>(request.TipoPessoa);
        pessoa.CpfCnpj = request.CpfCnpj;
        pessoa.RazaoSocial = request.RazaoSocial;
        pessoa.NomeFantasia = request.NomeFantasia;
        pessoa.InscricaoEstadual = request.InscricaoEstadual;
        pessoa.IndicadorIE = Enum.Parse<IndicadorIE>(request.IndicadorIE);
        pessoa.IsCliente = request.IsCliente;
        pessoa.IsFornecedor = request.IsFornecedor;
        pessoa.Email = request.Email;
        pessoa.Telefone = request.Telefone;

        if (request.Endereco is not null)
        {
            if (pessoa.Endereco is not null)
            {
                pessoa.Endereco.Logradouro = request.Endereco.Logradouro;
                pessoa.Endereco.Numero = request.Endereco.Numero;
                pessoa.Endereco.Complemento = request.Endereco.Complemento;
                pessoa.Endereco.Bairro = request.Endereco.Bairro;
                pessoa.Endereco.Cep = request.Endereco.Cep;
                pessoa.Endereco.Cidade = request.Endereco.Cidade;
                pessoa.Endereco.Uf = request.Endereco.Uf;
                pessoa.Endereco.CodigoIbge = request.Endereco.CodigoIbge;
            }
            else
            {
                pessoa.Endereco = new Endereco
                {
                    PessoaId = pessoa.Id,
                    Logradouro = request.Endereco.Logradouro,
                    Numero = request.Endereco.Numero,
                    Complemento = request.Endereco.Complemento,
                    Bairro = request.Endereco.Bairro,
                    Cep = request.Endereco.Cep,
                    Cidade = request.Endereco.Cidade,
                    Uf = request.Endereco.Uf,
                    CodigoIbge = request.Endereco.CodigoIbge,
                };
            }
        }

        await _context.SaveChangesAsync();
        return MapToResponse(pessoa);
    }

    public async Task DeleteAsync(Guid id)
    {
        var pessoa = await _context.Pessoas.FirstOrDefaultAsync(p => p.Id == id)
            ?? throw new KeyNotFoundException($"Pessoa com ID '{id}' não encontrada.");

        pessoa.Ativo = false;
        await _context.SaveChangesAsync();
    }

    private static PessoaResponse MapToResponse(Pessoa p) => new()
    {
        Id = p.Id,
        TipoPessoa = p.TipoPessoa.ToString(),
        CpfCnpj = p.CpfCnpj,
        RazaoSocial = p.RazaoSocial,
        NomeFantasia = p.NomeFantasia,
        InscricaoEstadual = p.InscricaoEstadual,
        IndicadorIE = p.IndicadorIE.ToString(),
        IsCliente = p.IsCliente,
        IsFornecedor = p.IsFornecedor,
        Email = p.Email,
        Telefone = p.Telefone,
        Ativo = p.Ativo,
        Endereco = p.Endereco is not null ? new EnderecoResponse
        {
            Id = p.Endereco.Id,
            Logradouro = p.Endereco.Logradouro,
            Numero = p.Endereco.Numero,
            Complemento = p.Endereco.Complemento,
            Bairro = p.Endereco.Bairro,
            Cep = p.Endereco.Cep,
            Cidade = p.Endereco.Cidade,
            Uf = p.Endereco.Uf,
            CodigoIbge = p.Endereco.CodigoIbge,
        } : null,
        CriadoEm = p.CriadoEm,
        AtualizadoEm = p.AtualizadoEm,
    };
}
