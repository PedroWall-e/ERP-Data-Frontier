
using ACBrLib.Core;
using ACBrLib.Core.DFe;
using ACBrLib.Core.NFe;
using ACBrLib.NFe;
using DataFrontier.Application.DTOs.Fiscal;
using DataFrontier.Application.Interfaces;
using DataFrontier.Domain.Entities;
using DataFrontier.Domain.Enums;
using DataFrontier.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DataFrontier.Infrastructure.Fiscal;

/// <summary>
/// Implementação do serviço fiscal utilizando ACBrLib.NFe.
/// 
/// Ciclo de vida:
/// - Registrado como Scoped no DI (1 instância por request).
/// - A instância nativa do ACBrNFe é criada sob demanda (lazy) e
///   protegida por SemaphoreSlim (ACBrLib NÃO é thread-safe).
/// - O Dispose() é chamado automaticamente pelo container ao final do request,
///   liberando a DLL nativa e evitando memory leaks.
/// 
/// Dependências nativas:
/// - A DLL nativa ACBrNFe64.dll (ou ACBrNFe32.dll) deve estar acessível
///   no diretório da aplicação ou no PATH do sistema.
/// - Schemas da NF-e (v4.00) devem estar no caminho configurado.
/// </summary>
public sealed class ACBrFiscalService : IFiscalService
{
    private readonly AppDbContext _context;
    private readonly ACBrNFeConfig _config;
    private readonly ILogger<ACBrFiscalService> _logger;
    private ACBrNFe? _nfe;
    private bool _disposed;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public ACBrFiscalService(
        AppDbContext context,
        IOptions<ACBrNFeConfig> config,
        ILogger<ACBrFiscalService> logger)
    {
        _context = context;
        _config = config.Value;
        _logger = logger;
    }

    // ─────────────────────────────────────────────────────────────
    // Inicialização lazy do ACBrNFe
    // ─────────────────────────────────────────────────────────────

    private ACBrNFe GetOrCreateNFe()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (_nfe is not null) return _nfe;

        _logger.LogInformation("Inicializando ACBrLib.NFe...");
        _nfe = new ACBrNFe();

        // ── Principal ────────────────────────────────────────────
        _nfe.ConfigGravarValor(ACBrSessao.Principal, "LogNivel", "4");

        // ── NFe ──────────────────────────────────────────────────
        // ACBrLib usa 0=Produção, 1=Homologação (diferente do padrão SEFAZ)
        var ambienteAcbr = _config.Ambiente == 1 ? "0" : "1";
        _nfe.ConfigGravarValor(ACBrSessao.NFe, "Ambiente", ambienteAcbr);
        _nfe.ConfigGravarValor(ACBrSessao.NFe, "PathSchemas", _config.PathSchemas);
        _nfe.ConfigGravarValor(ACBrSessao.NFe, "PathSalvar", _config.PathXml);

        // ── DFe (Certificado Digital) ────────────────────────────
        _nfe.ConfigGravarValor(ACBrSessao.DFe, "SSLCryptLib", "1");    // cryOpenSSL
        _nfe.ConfigGravarValor(ACBrSessao.DFe, "SSLHttpLib", "3");     // httpOpenSSL
        _nfe.ConfigGravarValor(ACBrSessao.DFe, "SSLXmlSignLib", "4");  // xsLibXml2
        _nfe.ConfigGravarValor(ACBrSessao.DFe, "ArquivoPFX", _config.Certificado.Caminho);
        _nfe.ConfigGravarValor(ACBrSessao.DFe, "Senha", _config.Certificado.Senha);

        // ── DANFE ────────────────────────────────────────────────
        _nfe.ConfigGravarValor(ACBrSessao.DANFE, "PathPDF", _config.PathPdf);
        _nfe.ConfigGravarValor(ACBrSessao.DANFE, "MostraPreview", "0");
        _nfe.ConfigGravarValor(ACBrSessao.DANFE, "MostraStatus", "0");

        _nfe.ConfigGravar();
        _logger.LogInformation(
            "ACBrLib.NFe inicializado — Ambiente={Ambiente}, Cert={Cert}",
            _config.Ambiente == 1 ? "Produção" : "Homologação",
            _config.Certificado.Caminho);

        return _nfe;
    }

    // ─────────────────────────────────────────────────────────────
    // Emissão de NF-e
    // ─────────────────────────────────────────────────────────────

    public async Task<FiscalResponseDTO> EmitirNFeAsync(Guid pedidoId)
    {
        await _semaphore.WaitAsync();
        try
        {
            // 1 ── Carrega o pedido com toda a árvore de navegação ──
            var pedido = await _context.Pedidos
                .Include(p => p.Pessoa)
                    .ThenInclude(p => p.Endereco)
                .Include(p => p.Itens)
                    .ThenInclude(i => i.Produto)
                .Include(p => p.Itens)
                    .ThenInclude(i => i.Impostos)
                .FirstOrDefaultAsync(p => p.Id == pedidoId)
                ?? throw new KeyNotFoundException($"Pedido '{pedidoId}' não encontrado.");

            // 2 ── Validações de negócio ─────────────────────────────
            if (pedido.Status != StatusPedido.Confirmado)
                throw new InvalidOperationException(
                    $"Apenas pedidos 'Confirmado' podem ser faturados. Status atual: {pedido.Status}");

            if (pedido.Pessoa.Endereco is null)
                throw new InvalidOperationException(
                    $"O cliente '{pedido.Pessoa.RazaoSocial}' não possui endereço cadastrado.");

            if (!pedido.Itens.Any())
                throw new InvalidOperationException("O pedido não possui itens.");

            // 3 ── Gera número sequencial da NF-e ────────────────────
            var ultimoNumero = await _context.Pedidos
                .Where(p => p.NumeroNfe.HasValue)
                .MaxAsync(p => (int?)p.NumeroNfe) ?? 0;
            var numeroNfe = ultimoNumero + 1;

            // 4 ── Mapeamento Pedido → INI (ACBrLib) ─────────────────
            var ini = NFeMapper.MapToINI(pedido, _config, numeroNfe);
            _logger.LogDebug("INI gerado para NF-e #{Numero}:\n{INI}", numeroNfe, ini);

            // 5 ── Operações ACBrLib (assinatura + transmissão) ───────
            var nfe = GetOrCreateNFe();
            nfe.LimparLista();
            nfe.CarregarINI(ini);
            nfe.Assinar();

            var lote = (int)(DateTime.UtcNow.Ticks % 999_999) + 1;
            var resultado = nfe.Enviar(lote, false, true); // síncrono

            _logger.LogInformation(
                "Retorno SEFAZ (lote {Lote}): CStat={CStat}, XMotivo={XMotivo}",
                lote, resultado.Retorno.CStat, resultado.Retorno.XMotivo);

            // 6 ── Extrai Chave de Acesso e Protocolo da resposta ─────
            var chaveAcesso = resultado.Retorno.ChaveDFe;
            var protocolo = resultado.Retorno.Protocolo ?? resultado.Envio.NProt;
            var cStat = resultado.Retorno.CStat;

            // CStat 100 = Autorizado, 150 = Autorizado fora de prazo
            if (cStat != 100 && cStat != 150)
            {
                _logger.LogWarning(
                    "SEFAZ rejeitou a NF-e: CStat={CStat}, Motivo={Motivo}",
                    cStat, resultado.Retorno.XMotivo);
                return new FiscalResponseDTO
                {
                    Sucesso = false,
                    CodigoRetorno = cStat,
                    Mensagem = $"SEFAZ rejeitou: [{cStat}] {resultado.Retorno.XMotivo}",
                    IniGerado = ini
                };
            }

            if (string.IsNullOrEmpty(chaveAcesso))
            {
                _logger.LogWarning("NF-e autorizada mas chave de acesso não retornada.");
                return new FiscalResponseDTO
                {
                    Sucesso = false,
                    Mensagem = $"NF-e autorizada (CStat={cStat}) mas chave de acesso não retornada.",
                    IniGerado = ini
                };
            }

            // 7 ── Gera PDF do DANFE ─────────────────────────────────
            string? caminhoPdf = null;
            try
            {
                nfe.ImprimirPDF();
                caminhoPdf = Path.Combine(_config.PathPdf, $"{chaveAcesso}-nfe.pdf");
                if (!File.Exists(caminhoPdf))
                    caminhoPdf = Directory.GetFiles(_config.PathPdf, $"*{chaveAcesso}*.pdf")
                        .FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Erro ao gerar PDF do DANFE — continua sem PDF.");
            }

            // 8 ── Atualiza o Pedido no banco ────────────────────────
            pedido.Status = StatusPedido.Faturado;
            pedido.NumeroNfe = numeroNfe;
            pedido.ChaveAcessoNfe = chaveAcesso;
            pedido.CaminhoPdfDanfe = caminhoPdf;
            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "NF-e #{Numero} emitida — Chave: {Chave}, Protocolo: {Proto}",
                numeroNfe, chaveAcesso, protocolo);

            return new FiscalResponseDTO
            {
                Sucesso = true,
                NumeroNfe = numeroNfe,
                ChaveAcesso = chaveAcesso,
                Protocolo = protocolo,
                CaminhoPdf = caminhoPdf,
                CaminhoXml = Path.Combine(_config.PathXml, $"{chaveAcesso}-nfe.xml"),
                Mensagem = "NF-e emitida com sucesso!"
            };
        }
        catch (Exception ex) when (ex is not KeyNotFoundException and not InvalidOperationException)
        {
            _logger.LogError(ex, "Erro não esperado ao emitir NF-e para pedido {PedidoId}", pedidoId);
            return new FiscalResponseDTO
            {
                Sucesso = false,
                Mensagem = $"Erro ao emitir NF-e: {ex.Message}",
                CodigoRetorno = -1
            };
        }
        finally
        {
            _semaphore.Release();
        }
    }

    // ─────────────────────────────────────────────────────────────
    // IDisposable — libera recursos nativos do ACBrLib
    // ─────────────────────────────────────────────────────────────

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        try
        {
            _nfe?.Dispose();
            _logger.LogDebug("ACBrLib.NFe disposed.");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Erro ao fazer Dispose do ACBrLib.NFe.");
        }

        _semaphore.Dispose();
    }
}
