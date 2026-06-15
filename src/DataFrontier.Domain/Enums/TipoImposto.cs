namespace DataFrontier.Domain.Enums;

/// <summary>
/// Tipos de tributos suportados pelo motor fiscal.
/// Abrange tanto o regime tributário atual quanto os novos tributos
/// da Reforma Tributária brasileira (EC 132/2023, LC 214/2025).
/// </summary>
public enum TipoImposto
{
    // ── Regime Atual ──────────────────────────────────────────

    /// <summary>Imposto sobre Circulação de Mercadorias e Serviços (estadual).</summary>
    ICMS = 1,

    /// <summary>ICMS por Substituição Tributária.</summary>
    ICMS_ST = 2,

    /// <summary>Imposto sobre Produtos Industrializados (federal).</summary>
    IPI = 3,

    /// <summary>Programa de Integração Social (federal).</summary>
    PIS = 4,

    /// <summary>Contribuição para Financiamento da Seguridade Social (federal).</summary>
    COFINS = 5,

    /// <summary>Imposto Sobre Serviços (municipal).</summary>
    ISS = 6,

    // ── Reforma Tributária (2026-2033) ────────────────────────

    /// <summary>
    /// Contribuição sobre Bens e Serviços (federal).
    /// Substitui PIS/COFINS a partir de 2027.
    /// </summary>
    CBS = 10,

    /// <summary>
    /// Imposto sobre Bens e Serviços (estadual/municipal).
    /// Substitui ICMS e ISS gradualmente entre 2029-2032.
    /// </summary>
    IBS = 11,

    /// <summary>
    /// Imposto Seletivo — incide sobre bens e serviços prejudiciais
    /// à saúde ou ao meio ambiente.
    /// </summary>
    IS = 12,

    // ── Extensibilidade ──────────────────────────────────────

    /// <summary>Tipo de tributo não categorizado. Usar MetadadosAdicionais para detalhes.</summary>
    Outro = 99
}
