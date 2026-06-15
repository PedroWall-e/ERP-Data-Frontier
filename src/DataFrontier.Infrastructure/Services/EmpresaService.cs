using System.Security.Cryptography.X509Certificates;
using DataFrontier.Application.DTOs.Empresa;
using DataFrontier.Application.Interfaces;
using DataFrontier.Domain.Entities;
using DataFrontier.Infrastructure.Persistence;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;

namespace DataFrontier.Infrastructure.Services;

public class EmpresaService : IEmpresaService
{
    private readonly AppDbContext _context;
    private readonly IDataProtector _protector;

    public EmpresaService(AppDbContext context, IDataProtectionProvider dpProvider)
    {
        _context = context;
        _protector = dpProvider.CreateProtector("DataFrontier.Certificado");
    }

    public async Task<ConfiguracaoEmpresaResponse> GetAsync()
    {
        var config = await _context.ConfiguracoesEmpresa.FirstOrDefaultAsync();
        return config is null ? new ConfiguracaoEmpresaResponse() : MapToResponse(config);
    }

    public async Task<ConfiguracaoEmpresaResponse> SaveAsync(ConfiguracaoEmpresaRequest request)
    {
        var config = await _context.ConfiguracoesEmpresa.FirstOrDefaultAsync();

        if (config is null)
        {
            config = new ConfiguracaoEmpresa();
            _context.ConfiguracoesEmpresa.Add(config);
        }

        // Map request to entity
        config.RazaoSocial = request.RazaoSocial;
        config.NomeFantasia = request.NomeFantasia;
        config.Cnpj = request.Cnpj;
        config.InscricaoEstadual = request.InscricaoEstadual;
        config.InscricaoMunicipal = request.InscricaoMunicipal;
        config.CRT = request.CRT;
        config.Logradouro = request.Logradouro;
        config.Numero = request.Numero;
        config.Complemento = request.Complemento;
        config.Bairro = request.Bairro;
        config.CodigoIbge = request.CodigoIbge;
        config.Municipio = request.Municipio;
        config.Uf = request.Uf;
        config.Cep = request.Cep;
        config.Telefone = request.Telefone;
        config.Email = request.Email;
        config.AmbienteFiscal = request.AmbienteFiscal;
        config.SerieNfe = request.SerieNfe;
        config.SerieNfse = request.SerieNfse;
        config.InterClientId = request.InterClientId;

        if (!string.IsNullOrEmpty(request.InterClientSecret))
            config.InterClientSecret = _protector.Protect(request.InterClientSecret);

        config.AtualizadoEm = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return MapToResponse(config);
    }

    public async Task<ConfiguracaoEmpresaResponse> UploadCertificadoAsync(Stream arquivoStream, string fileName, string senha)
    {
        using var ms = new MemoryStream();
        await arquivoStream.CopyToAsync(ms);
        var pfxBytes = ms.ToArray();

        if (pfxBytes.Length == 0)
            throw new InvalidOperationException("Arquivo do certificado está vazio.");

        // Valida o certificado
        try
        {
            using var cert = new X509Certificate2(pfxBytes, senha);
            var config = await GetOrCreateAsync();
            config.CertificadoPfx = pfxBytes;
            config.CertificadoSenha = _protector.Protect(senha);
            config.CertificadoValidade = cert.NotAfter.ToUniversalTime();
            config.CertificadoNome = fileName;
            config.AtualizadoEm = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return MapToResponse(config);
        }
        catch (Exception ex) when (ex is not InvalidOperationException)
        {
            throw new InvalidOperationException(
                $"Certificado inválido ou senha incorreta: {ex.Message}");
        }
    }

    public async Task RemoverCertificadoAsync()
    {
        var config = await _context.ConfiguracoesEmpresa.FirstOrDefaultAsync();
        if (config is null) return;

        config.CertificadoPfx = null;
        config.CertificadoSenha = null;
        config.CertificadoValidade = null;
        config.CertificadoNome = null;
        config.AtualizadoEm = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }

    private async Task<ConfiguracaoEmpresa> GetOrCreateAsync()
    {
        var config = await _context.ConfiguracoesEmpresa.FirstOrDefaultAsync();
        if (config is not null) return config;

        config = new ConfiguracaoEmpresa();
        _context.ConfiguracoesEmpresa.Add(config);
        return config;
    }

    private static ConfiguracaoEmpresaResponse MapToResponse(ConfiguracaoEmpresa c) => new()
    {
        Id = c.Id,
        RazaoSocial = c.RazaoSocial,
        NomeFantasia = c.NomeFantasia,
        Cnpj = c.Cnpj,
        InscricaoEstadual = c.InscricaoEstadual,
        InscricaoMunicipal = c.InscricaoMunicipal,
        CRT = c.CRT,
        Logradouro = c.Logradouro,
        Numero = c.Numero,
        Complemento = c.Complemento,
        Bairro = c.Bairro,
        CodigoIbge = c.CodigoIbge,
        Municipio = c.Municipio,
        Uf = c.Uf,
        Cep = c.Cep,
        Telefone = c.Telefone,
        Email = c.Email,
        TemCertificado = c.CertificadoPfx is not null && c.CertificadoPfx.Length > 0,
        CertificadoNome = c.CertificadoNome,
        CertificadoValidade = c.CertificadoValidade,
        AmbienteFiscal = c.AmbienteFiscal,
        SerieNfe = c.SerieNfe,
        SerieNfse = c.SerieNfse,
        TemInterConfig = !string.IsNullOrEmpty(c.InterClientId),
        InterClientId = c.InterClientId,
        CriadoEm = c.CriadoEm,
        AtualizadoEm = c.AtualizadoEm,
    };
}
