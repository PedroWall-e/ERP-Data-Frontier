using System.Globalization;
using System.Text;
using DataFrontier.Domain.Entities;
using DataFrontier.Domain.Enums;

namespace DataFrontier.Infrastructure.Fiscal;

/// <summary>
/// Converte um Pedido em uma string INI compatível com ACBrLib.NFe.
/// O formato INI é a interface padrão do ACBr para construção de documentos fiscais.
/// Ref: https://acbr.sourceforge.io/ACBrLib/ACBrNFeCarregarINI.html
/// </summary>
public static class NFeMapper
{
    private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;

    /// <summary>
    /// Gera o conteúdo INI completo da NF-e a partir do pedido.
    /// </summary>
    public static string MapToINI(Pedido pedido, ACBrNFeConfig config, int numeroNfe)
    {
        var sb = new StringBuilder(4096);
        var pessoa = pedido.Pessoa;
        var endereco = pessoa.Endereco;
        var emit = config.Emitente;
        var dtEmissao = pedido.DataEmissao.ToString("yyyy-MM-ddTHH:mm:sszzz");

        var ufEmit = emit.UF;
        var ufDest = endereco?.Uf ?? ufEmit;
        var operacaoInterna = string.Equals(ufEmit, ufDest, StringComparison.OrdinalIgnoreCase);
        var cfop = operacaoInterna ? "5102" : "6102";

        // ── infNFe ───────────────────────────────────────────────
        sb.AppendLine("[infNFe]");
        sb.AppendLine("versao=4.00");
        sb.AppendLine();

        // ── ide ──────────────────────────────────────────────────
        sb.AppendLine("[ide]");
        sb.AppendLine($"cUF={UfToCodigo(ufEmit)}");
        sb.AppendLine("natOp=VENDA DE MERCADORIA");
        sb.AppendLine("mod=55");
        sb.AppendLine(string.Create(Inv, $"serie={config.Serie}"));
        sb.AppendLine(string.Create(Inv, $"nNF={numeroNfe}"));
        sb.AppendLine($"dhEmi={dtEmissao}");
        sb.AppendLine($"dhSaiEnt={dtEmissao}");
        sb.AppendLine("tpNF=1");         // 1=saída
        sb.AppendLine($"idDest={(operacaoInterna ? 1 : 2)}");
        sb.AppendLine($"cMunFG={emit.CodigoIBGE}");
        sb.AppendLine("tpImp=1");         // DANFE retrato
        sb.AppendLine("tpEmis=1");        // emissão normal
        sb.AppendLine("finNFe=1");        // NF-e normal
        sb.AppendLine("indFinal=1");      // consumidor final
        sb.AppendLine("indPres=1");       // presencial
        sb.AppendLine("procEmi=0");       // emissão por aplicativo
        sb.AppendLine("verProc=DataFrontier ERP 1.0");
        sb.AppendLine();

        // ── emit ─────────────────────────────────────────────────
        sb.AppendLine("[emit]");
        sb.AppendLine($"CNPJ={LimparDoc(emit.CNPJ)}");
        sb.AppendLine($"xNome={emit.RazaoSocial}");
        sb.AppendLine($"xFant={emit.NomeFantasia}");
        sb.AppendLine($"IE={emit.IE}");
        sb.AppendLine(string.Create(Inv, $"CRT={emit.CRT}"));
        sb.AppendLine();

        sb.AppendLine("[emit.enderEmit]");
        sb.AppendLine($"xLgr={emit.Logradouro}");
        sb.AppendLine($"nro={emit.Numero}");
        if (!string.IsNullOrWhiteSpace(emit.Complemento))
            sb.AppendLine($"xCpl={emit.Complemento}");
        sb.AppendLine($"xBairro={emit.Bairro}");
        sb.AppendLine($"cMun={emit.CodigoIBGE}");
        sb.AppendLine($"xMun={emit.Municipio}");
        sb.AppendLine($"UF={emit.UF}");
        sb.AppendLine($"CEP={LimparDoc(emit.CEP)}");
        sb.AppendLine("cPais=1058");
        sb.AppendLine("xPais=Brasil");
        if (!string.IsNullOrWhiteSpace(emit.Telefone))
            sb.AppendLine($"fone={LimparDoc(emit.Telefone)}");
        sb.AppendLine();

        // ── dest ─────────────────────────────────────────────────
        sb.AppendLine("[dest]");
        if (pessoa.TipoPessoa == TipoPessoa.Juridica)
            sb.AppendLine($"CNPJ={LimparDoc(pessoa.CpfCnpj)}");
        else
            sb.AppendLine($"CPF={LimparDoc(pessoa.CpfCnpj)}");
        sb.AppendLine($"xNome={pessoa.RazaoSocial}");
        sb.AppendLine(string.Create(Inv, $"indIEDest={(int)pessoa.IndicadorIE}"));
        if (!string.IsNullOrWhiteSpace(pessoa.InscricaoEstadual)
            && pessoa.IndicadorIE == IndicadorIE.ContribuinteICMS)
            sb.AppendLine($"IE={pessoa.InscricaoEstadual}");
        if (!string.IsNullOrWhiteSpace(pessoa.Email))
            sb.AppendLine($"email={pessoa.Email}");
        sb.AppendLine();

        if (endereco is not null)
        {
            sb.AppendLine("[dest.enderDest]");
            sb.AppendLine($"xLgr={endereco.Logradouro}");
            sb.AppendLine($"nro={endereco.Numero}");
            if (!string.IsNullOrWhiteSpace(endereco.Complemento))
                sb.AppendLine($"xCpl={endereco.Complemento}");
            sb.AppendLine($"xBairro={endereco.Bairro}");
            sb.AppendLine($"cMun={endereco.CodigoIbge}");
            sb.AppendLine($"xMun={endereco.Cidade}");
            sb.AppendLine($"UF={endereco.Uf}");
            sb.AppendLine($"CEP={LimparDoc(endereco.Cep)}");
            sb.AppendLine($"cPais={endereco.CodigoPais}");
            sb.AppendLine($"xPais={endereco.Pais}");
            if (!string.IsNullOrWhiteSpace(pessoa.Telefone))
                sb.AppendLine($"fone={LimparDoc(pessoa.Telefone)}");
            sb.AppendLine();
        }

        // ── det (itens) ──────────────────────────────────────────
        decimal vBC = 0, vICMS = 0, vPIS = 0, vCOFINS = 0;
        var itensOrdenados = pedido.Itens.ToList();

        for (int i = 0; i < itensOrdenados.Count; i++)
        {
            var item = itensOrdenados[i];
            var idx = (i + 1).ToString("D3");
            var ncm = item.Produto?.CodigoNcm ?? "00000000";
            var vProd = item.ValorUnitario * item.Quantidade;

            sb.AppendLine($"[det{idx}]");
            sb.AppendLine(string.Create(Inv, $"nItem={i + 1}"));
            sb.AppendLine();

            sb.AppendLine($"[det{idx}.prod]");
            sb.AppendLine($"cProd={item.ProdutoCodigo}");
            sb.AppendLine("cEAN=SEM GTIN");
            sb.AppendLine($"xProd={item.ProdutoNome}");
            sb.AppendLine($"NCM={ncm}");
            sb.AppendLine($"CFOP={cfop}");
            sb.AppendLine("uCom=UN");
            sb.AppendLine(string.Create(Inv, $"qCom={item.Quantidade:F4}"));
            sb.AppendLine(string.Create(Inv, $"vUnCom={item.ValorUnitario:F10}"));
            sb.AppendLine(string.Create(Inv, $"vProd={vProd:F2}"));
            sb.AppendLine("cEANTrib=SEM GTIN");
            sb.AppendLine("uTrib=UN");
            sb.AppendLine(string.Create(Inv, $"qTrib={item.Quantidade:F4}"));
            sb.AppendLine(string.Create(Inv, $"vUnTrib={item.ValorUnitario:F10}"));
            if (item.ValorDesconto > 0)
                sb.AppendLine(string.Create(Inv, $"vDesc={item.ValorDesconto:F2}"));
            sb.AppendLine("indTot=1");
            sb.AppendLine();

            // ── Impostos do item ─────────────────────────────────
            var temICMS = false;
            var temPIS = false;
            var temCOFINS = false;

            foreach (var imp in item.Impostos)
            {
                var nome = imp.NomeImposto.ToUpperInvariant().Trim();
                switch (nome)
                {
                    case "ICMS":
                        temICMS = true;
                        sb.AppendLine($"[det{idx}.Imposto.ICMS.ICMS00]");
                        sb.AppendLine("orig=0");
                        sb.AppendLine("CST=00");
                        sb.AppendLine("modBC=3");
                        sb.AppendLine(string.Create(Inv, $"vBC={imp.BaseCalculo:F2}"));
                        sb.AppendLine(string.Create(Inv, $"pICMS={imp.Aliquota:F4}"));
                        sb.AppendLine(string.Create(Inv, $"vICMS={imp.ValorImposto:F2}"));
                        sb.AppendLine();
                        vBC += imp.BaseCalculo;
                        vICMS += imp.ValorImposto;
                        break;

                    case "PIS":
                        temPIS = true;
                        sb.AppendLine($"[det{idx}.Imposto.PIS.PISAliq]");
                        sb.AppendLine("CST=01");
                        sb.AppendLine(string.Create(Inv, $"vBC={imp.BaseCalculo:F2}"));
                        sb.AppendLine(string.Create(Inv, $"pPIS={imp.Aliquota:F4}"));
                        sb.AppendLine(string.Create(Inv, $"vPIS={imp.ValorImposto:F2}"));
                        sb.AppendLine();
                        vPIS += imp.ValorImposto;
                        break;

                    case "COFINS":
                        temCOFINS = true;
                        sb.AppendLine($"[det{idx}.Imposto.COFINS.COFINSAliq]");
                        sb.AppendLine("CST=01");
                        sb.AppendLine(string.Create(Inv, $"vBC={imp.BaseCalculo:F2}"));
                        sb.AppendLine(string.Create(Inv, $"pCOFINS={imp.Aliquota:F4}"));
                        sb.AppendLine(string.Create(Inv, $"vCOFINS={imp.ValorImposto:F2}"));
                        sb.AppendLine();
                        vCOFINS += imp.ValorImposto;
                        break;

                    // IBS/CBS — Reforma Tributária (mapeamento futuro)
                    default:
                        break;
                }
            }

            // Fallback: seções obrigatórias mesmo sem imposto
            if (!temICMS)
            {
                sb.AppendLine($"[det{idx}.Imposto.ICMS.ICMS00]");
                sb.AppendLine("orig=0");
                sb.AppendLine("CST=00");
                sb.AppendLine("modBC=3");
                sb.AppendLine(string.Create(Inv, $"vBC={item.ValorTotal:F2}"));
                sb.AppendLine("pICMS=0.0000");
                sb.AppendLine("vICMS=0.00");
                sb.AppendLine();
            }
            if (!temPIS)
            {
                sb.AppendLine($"[det{idx}.Imposto.PIS.PISOutr]");
                sb.AppendLine("CST=99");
                sb.AppendLine("vBC=0.00");
                sb.AppendLine("pPIS=0.0000");
                sb.AppendLine("vPIS=0.00");
                sb.AppendLine();
            }
            if (!temCOFINS)
            {
                sb.AppendLine($"[det{idx}.Imposto.COFINS.COFINSOutr]");
                sb.AppendLine("CST=99");
                sb.AppendLine("vBC=0.00");
                sb.AppendLine("pCOFINS=0.0000");
                sb.AppendLine("vCOFINS=0.00");
                sb.AppendLine();
            }
        }

        // ── total ────────────────────────────────────────────────
        sb.AppendLine("[total]");
        sb.AppendLine(string.Create(Inv, $"vBC={vBC:F2}"));
        sb.AppendLine(string.Create(Inv, $"vICMS={vICMS:F2}"));
        sb.AppendLine("vICMSDeson=0.00");
        sb.AppendLine("vFCP=0.00");
        sb.AppendLine("vBCST=0.00");
        sb.AppendLine("vST=0.00");
        sb.AppendLine("vFCPST=0.00");
        sb.AppendLine("vFCPSTRet=0.00");
        sb.AppendLine(string.Create(Inv, $"vProd={pedido.ValorTotalProdutos:F2}"));
        sb.AppendLine("vFrete=0.00");
        sb.AppendLine("vSeg=0.00");
        sb.AppendLine(string.Create(Inv, $"vDesc={pedido.ValorTotalDesconto:F2}"));
        sb.AppendLine("vII=0.00");
        sb.AppendLine("vIPI=0.00");
        sb.AppendLine("vIPIDevol=0.00");
        sb.AppendLine(string.Create(Inv, $"vPIS={vPIS:F2}"));
        sb.AppendLine(string.Create(Inv, $"vCOFINS={vCOFINS:F2}"));
        sb.AppendLine("vOutro=0.00");
        sb.AppendLine(string.Create(Inv, $"vNF={pedido.ValorTotalPedido:F2}"));
        sb.AppendLine();

        // ── transp ───────────────────────────────────────────────
        sb.AppendLine("[transp]");
        sb.AppendLine("modFrete=9"); // 9=sem frete
        sb.AppendLine();

        // ── pag ──────────────────────────────────────────────────
        sb.AppendLine("[pag001]");
        sb.AppendLine("tPag=01"); // 01=dinheiro
        sb.AppendLine(string.Create(Inv, $"vPag={pedido.ValorTotalPedido:F2}"));
        sb.AppendLine();

        // ── infAdic ──────────────────────────────────────────────
        if (!string.IsNullOrWhiteSpace(pedido.Observacoes))
        {
            sb.AppendLine("[infAdic]");
            sb.AppendLine($"infCpl={pedido.Observacoes}");
            sb.AppendLine();
        }

        return sb.ToString();
    }

    // ─────────────────────────────────────────────────────────────
    // Helpers
    // ─────────────────────────────────────────────────────────────

    /// <summary>Remove caracteres não-numéricos (pontos, traços, barras).</summary>
    private static string LimparDoc(string? valor)
        => string.IsNullOrWhiteSpace(valor)
            ? string.Empty
            : new string(valor.Where(char.IsDigit).ToArray());

    /// <summary>Tabela IBGE: UF → Código numérico (2 dígitos).</summary>
    public static string UfToCodigo(string uf) => uf.ToUpperInvariant() switch
    {
        "AC" => "12", "AL" => "27", "AM" => "13", "AP" => "16",
        "BA" => "29", "CE" => "23", "DF" => "53", "ES" => "32",
        "GO" => "52", "MA" => "21", "MG" => "31", "MS" => "50",
        "MT" => "51", "PA" => "15", "PB" => "25", "PE" => "26",
        "PI" => "22", "PR" => "41", "RJ" => "33", "RN" => "24",
        "RO" => "11", "RR" => "14", "RS" => "43", "SC" => "42",
        "SE" => "28", "SP" => "35", "TO" => "17",
        _ => "35"
    };
}
