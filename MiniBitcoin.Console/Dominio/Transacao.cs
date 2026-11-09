namespace MiniBitcoin.Dominio;

public enum TipoTransacao { Compra, Venda }

public sealed record Transacao(
    TipoTransacao Tipo,
    decimal QuantidadeBtc,
    decimal Cotacao,
    decimal TotalBrl,
    DateTimeOffset Data);
