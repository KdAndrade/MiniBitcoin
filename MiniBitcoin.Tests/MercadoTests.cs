using MiniBitcoin.Dominio;

namespace MiniBitcoin.Tests;

public class MercadoTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(0.001)]
    public void RejeitaCotacaoInvalida(decimal cotacao) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => new Mercado(new Carteira(), cotacao));

    [Fact]
    public void CompraEVendaAtualizamSaldos()
    {
        var carteira = new Carteira(1000);
        var mercado = new Mercado(carteira, 100000);
        mercado.Comprar(0.005m);
        Assert.Equal(500, carteira.SaldoBrl);
        Assert.Equal(0.005m, carteira.SaldoBtc);
        mercado.AtualizarCotacao(200000);
        mercado.Vender(0.002m);
        Assert.Equal(900, carteira.SaldoBrl);
        Assert.Equal(0.003m, carteira.SaldoBtc);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(0.000000001)]
    [InlineData(2)]
    public void OperacoesInvalidasPreservamCarteira(decimal btc)
    {
        var carteira = new Carteira(100, 1);
        var mercado = new Mercado(carteira, 100000);
        Assert.ThrowsAny<ArgumentException>(() => mercado.Comprar(btc));
        Assert.ThrowsAny<ArgumentException>(() => mercado.Vender(btc));
        Assert.Equal(100, carteira.SaldoBrl);
        Assert.Equal(1, carteira.SaldoBtc);
    }

    [Fact]
    public void CotacaoInvalidaPreservaPrecoAnterior()
    {
        var mercado = new Mercado(new Carteira(), 100000);
        Assert.Throws<ArgumentOutOfRangeException>(() => mercado.AtualizarCotacao(0));
        Assert.Equal(100000, mercado.Cotacao);
    }

    [Fact]
    public void RejeitaOperacaoQueArredondaParaZero()
    {
        var carteira = new Carteira(10, 1);
        var mercado = new Mercado(carteira, 1);
        Assert.ThrowsAny<ArgumentException>(() => mercado.Comprar(0.00000001m));
        Assert.ThrowsAny<ArgumentException>(() => mercado.Vender(0.00000001m));
    }

    [Fact]
    public void ArredondaCentavosDeFormaExplicita()
    {
        var carteira = new Carteira(10);
        new Mercado(carteira, 1.50m).Comprar(0.01m);
        Assert.Equal(9.98m, carteira.SaldoBrl);
    }

    [Fact]
    public void OverflowNaoDebitaCarteira()
    {
        var carteira = new Carteira(decimal.MaxValue, 1);
        var mercado = new Mercado(carteira, 1);
        Assert.Throws<OverflowException>(() => mercado.Vender(1));
        Assert.Equal(1, carteira.SaldoBtc);
    }
}
