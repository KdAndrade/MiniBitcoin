using MiniBitcoin.Dominio;

namespace MiniBitcoin.Tests;

public class CarteiraTests
{
    [Theory]
    [InlineData(-1, 0)]
    [InlineData(0, -1)]
    [InlineData(1, 0.000000001)]
    [InlineData(0.001, 0)]
    public void RejeitaSaldosIniciaisInvalidos(decimal brl, decimal btc) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => new Carteira(brl, btc));

    [Fact]
    public void MantemSaldosIniciaisESatoshis()
    {
        var carteira = new Carteira(100, 0.00000001m);
        Assert.Equal(100, carteira.SaldoBrl);
        Assert.Equal(0.00000001m, carteira.SaldoBtc);
        Assert.Equal(1, carteira.SaldoSatoshis);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(101)]
    [InlineData(0.001)]
    public void RejeitaDebitoInvalidoSemAlterarSaldo(decimal valor)
    {
        var carteira = new Carteira(100, 1);
        Assert.ThrowsAny<ArgumentException>(() => carteira.DebitarBrl(valor));
        Assert.Equal(100, carteira.SaldoBrl);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(2)]
    [InlineData(0.000000001)]
    public void RejeitaDebitoBtcInvalido(decimal valor)
    {
        var carteira = new Carteira(100, 1);
        Assert.ThrowsAny<ArgumentException>(() => carteira.DebitarBtc(valor));
        Assert.Equal(1, carteira.SaldoBtc);
    }

    [Fact]
    public void PermiteDebitarTodoOSaldo()
    {
        var carteira = new Carteira(100, 1);
        carteira.DebitarBrl(100);
        carteira.DebitarBtc(1);
        Assert.Equal(0, carteira.SaldoBrl);
        Assert.Equal(0, carteira.SaldoBtc);
    }
}
