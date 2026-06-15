namespace DataFrontier.Domain.Enums;

/// <summary>
/// Categoria de competência do tributo, utilizada para classificação
/// e roteamento de arrecadação (incluindo Split Payment na reforma).
/// </summary>
public enum CategoriaTributo
{
    /// <summary>Tributo de competência federal (ex: IPI, PIS, COFINS, CBS).</summary>
    Federal = 1,

    /// <summary>Tributo de competência estadual (ex: ICMS, parcela estadual do IBS).</summary>
    Estadual = 2,

    /// <summary>Tributo de competência municipal (ex: ISS, parcela municipal do IBS).</summary>
    Municipal = 3,

    /// <summary>Imposto Seletivo (competência federal, incidência específica).</summary>
    Seletivo = 4
}
