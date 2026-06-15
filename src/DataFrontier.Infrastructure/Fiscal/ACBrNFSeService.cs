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
/// Implementação do serviço de NFS-e usando ACBrLib.NFSe (Padrão Nacional RFB).
/// NOTA: A emissão real requer a DLL nativa ACBrNFSe64.dll.
/// Em homologação, gera o INI para validação.
/// </summary>
public class ACBrNFSeService : INFSeService
{
    private readonly AppDbContext _context;
    private readonly ACBrNFSeConfig _config;
    private readonly ILogger<ACBrNFSeService> _logger;
    private static readonly SemaphoreSlim _semaphore = new(1, 1);
    private bool _disposed;

    public ACBrNFSeService(
        AppDbContext context,
        IOptions<ACBrNFSeConfig> config,
        ILogger<ACBrNFSeService> logger)
    {
        _context = context;
        _config = config.Value;
        _logger = logger;
    }

    public async Task<FiscalResponseDTO> EmitirNFSeAsync(Guid pedidoId)
    {
        await _semaphore.WaitAsync();
        try
        {
            // 1. Carrega pedido completo
            var pedido = await _context.Pedidos
                .Include(p => p.Pessoa)
                    .ThenInclude(p => p.Endereco)
                .Include(p => p.Itens)
                    .ThenInclude(i => i.Produto)
                .FirstOrDefaultAsync(p => p.Id == pedidoId)
                ?? throw new InvalidOperationException($"Pedido {pedidoId} não encontrado.");

            if (pedido.Status != StatusPedido.Confirmado)
                return new FiscalResponseDTO { Sucesso = false, Mensagem = "Pedido não está Confirmado." };

            // Verifica se todos os itens são serviço
            var itensNaoServico = pedido.Itens.Where(i => !i.Produto.IsServico).ToList();
            if (itensNaoServico.Any())
                return new FiscalResponseDTO
                {
                    Sucesso = false,
                    Mensagem = $"Os seguintes itens não são serviços: {string.Join(", ", itensNaoServico.Select(i => i.Produto.Nome))}"
                };

            // 2. Carrega configuração da empresa
            var empresa = await _context.ConfiguracoesEmpresa.FirstOrDefaultAsync()
                ?? throw new InvalidOperationException("Configuração da empresa não encontrada. Configure em Configurações > Dados Gerais.");

            if (string.IsNullOrEmpty(empresa.InscricaoMunicipal))
                return new FiscalResponseDTO
                {
                    Sucesso = false,
                    Mensagem = "Inscrição Municipal não configurada. Necessária para NFS-e."
                };

            // 3. Gera próximo número
            var ultimoNumero = await _context.Pedidos
                .Where(p => p.NumeroNfse != null)
                .MaxAsync(p => (long?)p.NumeroNfse) ?? 0;
            var numero = (int)(ultimoNumero + 1);

            // 4. Gera o INI da NFS-e
            var iniNFSe = NFSeMapper.MapPedidoToIniNFSe(pedido, empresa, numero, empresa.SerieNfse);

            _logger.LogInformation("INI da NFS-e gerado para Pedido {PedidoId}:\n{Ini}", pedidoId, iniNFSe);

            // 5. Em homologação: simula o envio
            if (empresa.AmbienteFiscal == 2) // Homologação
            {
                _logger.LogWarning("HOMOLOGAÇÃO: NFS-e simulada para Pedido {PedidoId}", pedidoId);

                var codigoVerif = Guid.NewGuid().ToString("N")[..12].ToUpper();

                pedido.NumeroNfse = numero;
                pedido.CodigoVerificacaoNfse = codigoVerif;
                pedido.LinkNfseNacional = $"https://www.nfse.gov.br/ConsultaPublica?chave=HOMOLOG-{codigoVerif}";
                pedido.Status = StatusPedido.Faturado;

                await _context.SaveChangesAsync();

                return new FiscalResponseDTO
                {
                    Sucesso = true,
                    NumeroNfe = numero,
                    ChaveAcesso = codigoVerif,
                    Protocolo = $"SIM-{DateTime.UtcNow:yyyyMMddHHmmss}",
                    Mensagem = $"[HOMOLOGAÇÃO] NFS-e #{numero} simulada com sucesso.",
                    IniGerado = iniNFSe
                };
            }

            // 6. PRODUÇÃO — chamada real à ACBrLib.NFSe
            // TODO: Integrar com ACBrLib.NFSe quando DLLs disponíveis
            // var nfse = new ACBrNFSe();
            // nfse.ConfigGravarValor(SESSAO.NFSe, "Provedor", "Nacional");
            // ...

            return new FiscalResponseDTO
            {
                Sucesso = false,
                Mensagem = "Emissão em produção requer ACBrLib.NFSe nativo instalado."
            };
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }
}
