namespace DataFrontier.Domain.Enums;

/// <summary>
/// Indicador da Inscrição Estadual conforme tabela da NF-e (campo indIEDest).
/// </summary>
public enum IndicadorIE
{
    /// <summary>1 — Contribuinte ICMS (informar IE).</summary>
    ContribuinteICMS = 1,

    /// <summary>2 — Contribuinte isento de Inscrição.</summary>
    Isento = 2,

    /// <summary>9 — Não Contribuinte (consumidor final / PF).</summary>
    NaoContribuinte = 9
}
