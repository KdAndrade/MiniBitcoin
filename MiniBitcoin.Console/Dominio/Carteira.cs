namespace MiniBitcoin.Dominio;

public sealed class Carteira
{
    public decimal SaldoBrl { get; private set; }
    public decimal SaldoBtc { get; private set; }
    public decimal SaldoSatoshis => SaldoBtc * 100_000_000m;

    public Carteira(decimal saldoBrl = 0, decimal saldoBtc = 0)
    {
        Validar(saldoBrl, 2, true);
        Validar(saldoBtc, 8, true);
        SaldoBrl = saldoBrl;
        SaldoBtc = saldoBtc;
    }

    public void DebitarBrl(decimal valor)
    {
        Validar(valor, 2);
        if (valor > SaldoBrl) throw new ArgumentException("Saldo em BRL insuficiente.");
        SaldoBrl -= valor;
    }

    public void DebitarBtc(decimal valor)
    {
        Validar(valor, 8);
        if (valor > SaldoBtc) throw new ArgumentException("Saldo em BTC insuficiente.");
        SaldoBtc -= valor;
    }

    internal void CreditarBrl(decimal valor)
    {
        Validar(valor, 2);
        SaldoBrl += valor;
    }

    internal void CreditarBtc(decimal valor)
    {
        Validar(valor, 8);
        SaldoBtc += valor;
    }

    internal static void Validar(decimal valor, int casas, bool permiteZero = false)
    {
        if (valor < 0 || (!permiteZero && valor == 0) || decimal.Round(valor, casas) != valor)
            throw new ArgumentOutOfRangeException(nameof(valor), $"Valor inválido: use até {casas} casas decimais e um valor {(permiteZero ? "não negativo" : "positivo")}.");
    }
}
