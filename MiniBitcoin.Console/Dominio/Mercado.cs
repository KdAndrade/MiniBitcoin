namespace MiniBitcoin.Dominio;

public sealed class Mercado
{
    private readonly Carteira carteira;
    private readonly List<Transacao> historico = [];
    public IReadOnlyList<Transacao> Historico => historico.AsReadOnly();
    public decimal PatrimonioTotal => decimal.Round(
        carteira.SaldoBrl + carteira.SaldoBtc * Cotacao, 2, MidpointRounding.AwayFromZero);
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
        _ = carteira.SaldoBtc + quantidadeBtc;
        carteira.DebitarBrl(total);
        carteira.CreditarBtc(quantidadeBtc);
        historico.Add(new Transacao(TipoTransacao.Compra, quantidadeBtc, Cotacao, total, DateTimeOffset.UtcNow));
    }

    public void Vender(decimal quantidadeBtc)
    {
        var total = CalcularTotal(quantidadeBtc);
        _ = carteira.SaldoBrl + total;
        carteira.DebitarBtc(quantidadeBtc);
        carteira.CreditarBrl(total);
        historico.Add(new Transacao(TipoTransacao.Venda, quantidadeBtc, Cotacao, total, DateTimeOffset.UtcNow));
    }

    private decimal CalcularTotal(decimal quantidadeBtc)
    {
        Carteira.Validar(quantidadeBtc, 8);
        var total = decimal.Round(quantidadeBtc * Cotacao, 2, MidpointRounding.AwayFromZero);
        if (total == 0) throw new ArgumentException("O valor da operação deve ser de pelo menos R$ 0,01.");
        return total;
    }
}
