using DataFrontier.Domain.Enums;

namespace DataFrontier.Domain.Entities;

/// <summary>
/// Representa um tributo calculado sobre um item de documento fiscal.
/// 
/// Esta entidade implementa o padrão de abstração tributária que permite
/// a coexistência de regimes fiscais durante a Reforma Tributária brasileira (2026-2033):
/// - Regime atual: ICMS, ISS, IPI, PIS, COFINS
/// - Regime novo: CBS, IBS, IS (Imposto Seletivo)
/// 
/// Um mesmo item pode possuir múltiplos registros de impostos simultâneos,
/// permitindo, por exemplo, ICMS (alíquota reduzida) + IBS (alíquota crescente)
/// durante o período de transição (2029-2032).
/// 
/// O campo <see cref="MetadadosAdicionais"/> (JSONB no PostgreSQL) armazena
/// dados específicos de cada tributo sem necessidade de alteração no schema.
/// </summary>
public class ImpostoDoItem : BaseEntity
{
    /// <summary>
    /// FK para o item do documento fiscal ao qual este imposto se refere.
    /// Preparado para vinculação futura com a entidade ItemDocumentoFiscal.
    /// </summary>
    public Guid ItemDocumentoFiscalId { get; set; }

    /// <summary>
    /// Tipo do tributo aplicado (ICMS, IBS, CBS, etc.).
    /// </summary>
    public TipoImposto TipoImposto { get; set; }

    /// <summary>
    /// Código de Situação Tributária (CST), CSOSN, ou código equivalente
    /// para os novos tributos CBS/IBS. Ex: "00", "20", "41".
    /// </summary>
    public string? CodigoTributario { get; set; }

    /// <summary>
    /// Valor da base de cálculo do imposto em reais (BRL).
    /// </summary>
    public decimal BaseCalculo { get; set; }

    /// <summary>
    /// Alíquota aplicada, expressa como percentual (ex: 18.00 para 18%).
    /// </summary>
    public decimal Aliquota { get; set; }

    /// <summary>
    /// Valor calculado do imposto em reais (BRL).
    /// Fórmula padrão: BaseCalculo * (Aliquota / 100).
    /// </summary>
    public decimal ValorImposto { get; set; }

    /// <summary>
    /// Categoria do tributo para classificação e roteamento.
    /// </summary>
    public CategoriaTributo Categoria { get; set; }

    /// <summary>
    /// Dados adicionais específicos do tributo em formato JSON.
    /// Armazenado como JSONB no PostgreSQL para consultas indexáveis.
    /// 
    /// Exemplos de uso:
    /// - ICMS: {"origem": "0", "modalidadeBc": "3", "percentualReducaoBc": 33.33}
    /// - IBS: {"codigoMunicipio": "3550308", "splitPaymentInfo": {...}}
    /// </summary>
    public string? MetadadosAdicionais { get; set; }
}
