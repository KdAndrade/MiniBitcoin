namespace MiniBitcoin.Dominio;

public sealed class Mercado
{
    private readonly Carteira carteira;
    private readonly List<Transacao> historico = [];
    public IReadOnlyList<Transacao> Historico => historico.AsReadOnly();
    public decimal PatrimonioTotal => ArredondarReais(carteira.SaldoBrl + carteira.SaldoBtc * Cotacao);
    public decimal Cotacao { get; private set; }

    public Mercado(Carteira carteira, decimal cotacao)
    {
        this.carteira = carteira ?? throw new ArgumentNullException(nameof(carteira));
        AtualizarCotacao(cotacao);
    }

    public void AtualizarCotacao(decimal cotacao)
    {
        Carteira.Validar(cotacao, 2);
        Cotacao = cotacao;
    }

    public void Comprar(decimal quantidadeBtc)
    {
        var total = CalcularTotal(quantidadeBtc);
        // Valida também o crédito antes de debitar, preservando os saldos em caso de overflow.
        _ = carteira.SaldoBtc + quantidadeBtc;
        carteira.DebitarBrl(total);
        carteira.CreditarBtc(quantidadeBtc);
        Registrar(TipoTransacao.Compra, quantidadeBtc, total);
    }

    public void Vender(decimal quantidadeBtc)
    {
        var total = CalcularTotal(quantidadeBtc);
        _ = carteira.SaldoBrl + total;
        carteira.DebitarBtc(quantidadeBtc);
        carteira.CreditarBrl(total);
        Registrar(TipoTransacao.Venda, quantidadeBtc, total);
    }

    private decimal CalcularTotal(decimal quantidadeBtc)
    {
        Carteira.Validar(quantidadeBtc, 8);
        var total = ArredondarReais(quantidadeBtc * Cotacao);
        if (total == 0) throw new ArgumentException("O valor da operação deve ser de pelo menos R$ 0,01.");
        return total;
    }

    private void Registrar(TipoTransacao tipo, decimal quantidadeBtc, decimal totalBrl) =>
        historico.Add(new Transacao(tipo, quantidadeBtc, Cotacao, totalBrl, DateTimeOffset.UtcNow));

    private static decimal ArredondarReais(decimal valor) =>
        decimal.Round(valor, 2, MidpointRounding.AwayFromZero);
}
