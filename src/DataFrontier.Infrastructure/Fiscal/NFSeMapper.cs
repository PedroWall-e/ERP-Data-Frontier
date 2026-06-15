using System.Globalization;
using System.Text;
using DataFrontier.Domain.Entities;

namespace DataFrontier.Infrastructure.Fiscal;

/// <summary>
/// Mapper que converte um Pedido (de serviço) para o formato INI da ACBrLib.NFSe
/// seguindo o padrão DPS (Declaração de Prestação de Serviço) da NFS-e Nacional.
/// </summary>
public static class NFSeMapper
{
    public static string MapPedidoToIniNFSe(
        Pedido pedido,
        ConfiguracaoEmpresa empresa,
        int numero,
        int serie)
    {
        var ini = new StringBuilder();
        var cultura = CultureInfo.InvariantCulture;

        // ── Identificação do RPS ─────────────────────────────────
        ini.AppendLine("[IdentificacaoRps]");
        ini.AppendLine($"Numero={numero}");
        ini.AppendLine($"Serie={serie}");
        ini.AppendLine("Tipo=1"); // RPS
        ini.AppendLine($"DataEmissao={pedido.DataEmissao:yyyy-MM-dd}");
        ini.AppendLine($"Competencia={pedido.DataEmissao:yyyy-MM-dd}");
        ini.AppendLine();

        // ── Prestador (Emitente) ─────────────────────────────────
        ini.AppendLine("[Prestador]");
        ini.AppendLine($"CNPJ={empresa.Cnpj.Replace(".", "").Replace("/", "").Replace("-", "")}");
        ini.AppendLine($"InscricaoMunicipal={empresa.InscricaoMunicipal}");
        ini.AppendLine();

        // ── Tomador (Cliente) ────────────────────────────────────
        var tomador = pedido.Pessoa;
        var cpfCnpj = tomador.CpfCnpj.Replace(".", "").Replace("/", "").Replace("-", "");
        ini.AppendLine("[Tomador]");
        ini.AppendLine($"CpfCnpj={cpfCnpj}");
        ini.AppendLine($"RazaoSocial={tomador.RazaoSocial}");

        if (tomador.Endereco is not null)
        {
            var end = tomador.Endereco;
            ini.AppendLine($"Logradouro={end.Logradouro}");
            ini.AppendLine($"Numero={end.Numero}");
            ini.AppendLine($"Complemento={end.Complemento}");
            ini.AppendLine($"Bairro={end.Bairro}");
            ini.AppendLine($"CodigoMunicipio={end.CodigoIbge}");
            ini.AppendLine($"UF={end.Uf}");
            ini.AppendLine($"CEP={end.Cep?.Replace("-", "")}");
        }

        ini.AppendLine($"Email={tomador.Email}");
        ini.AppendLine($"Telefone={tomador.Telefone}");
        ini.AppendLine();

        // ── Serviço ──────────────────────────────────────────────
        ini.AppendLine("[Servico]");

        // Pega o código de serviço do primeiro item (todos devem ser do mesmo código)
        var primeiroItem = pedido.Itens.First();
        var produtoServico = primeiroItem.Produto;

        ini.AppendLine($"CodigoTributacaoMunicipio={produtoServico.CodigoServico}");
        ini.AppendLine($"CodigoCnae={produtoServico.CodigoCnae}");
        ini.AppendLine($"CodigoNbs={produtoServico.CodigoNbs}");
        ini.AppendLine($"Discriminacao={BuildDiscriminacao(pedido)}");
        ini.AppendLine($"CodigoMunicipio={empresa.CodigoIbge}");
        ini.AppendLine("ExigibilidadeISS=1"); // Exigível
        ini.AppendLine("MunicipioIncidencia=" + empresa.CodigoIbge);
        ini.AppendLine();

        // ── Valores ─────────────────────────────────────────────
        ini.AppendLine("[Valores]");
        ini.AppendLine($"ValorServicos={pedido.ValorTotalProdutos.ToString("F2", cultura)}");
        ini.AppendLine($"ValorDeducoes=0.00");
        ini.AppendLine($"ValorPis=0.00");
        ini.AppendLine($"ValorCofins=0.00");
        ini.AppendLine($"ValorInss=0.00");
        ini.AppendLine($"ValorIr=0.00");
        ini.AppendLine($"ValorCsll=0.00");
        ini.AppendLine($"ValorIss={pedido.ValorTotalImpostos.ToString("F2", cultura)}");
        ini.AppendLine($"Aliquota=0.00"); // Será calculado pelo município
        ini.AppendLine($"ValorLiquidoNfse={pedido.ValorTotalPedido.ToString("F2", cultura)}");
        ini.AppendLine($"DescontoCondicionado={pedido.ValorTotalDesconto.ToString("F2", cultura)}");
        ini.AppendLine();

        return ini.ToString();
    }

    private static string BuildDiscriminacao(Pedido pedido)
    {
        var sb = new StringBuilder();
        foreach (var item in pedido.Itens)
        {
            sb.Append($"{item.Produto.Nome} - Qtd: {item.Quantidade} x R$ {item.ValorUnitario:N2} = R$ {item.ValorTotal:N2}; ");
        }
        return sb.ToString().TrimEnd(' ', ';');
    }
}
