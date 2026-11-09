using MiniBitcoin.Interface;

namespace MiniBitcoin.Tests;

public class ConsoleTests
{
    [Fact]
    public void ExecutaFluxoDeCompraVendaECotacao()
    {
        using var entrada = new StringReader("1\n2\n3\n0,005\n6\n200000\n4\n0.002\n5\n2\n0\n");
        using var saida = new StringWriter();
        new AplicacaoConsole(entrada, saida).Executar();
        var texto = saida.ToString();
        Assert.Contains("Compra realizada", texto);
        Assert.Contains("Venda realizada", texto);
        Assert.Contains("0,00300000 BTC", texto);
        Assert.Contains("1.500,00", texto);
        Assert.Contains("Compra |", texto);
        Assert.Contains("Venda |", texto);
    }

    [Fact]
    public void TrataEntradaInvalidaESaldoInsuficiente()
    {
        using var entrada = new StringReader("x\n3\nabc\n3\n1\n4\n1\n6\n-1\n5\n0\n");
        using var saida = new StringWriter();
        new AplicacaoConsole(entrada, saida).Executar();
        var texto = saida.ToString();
        Assert.Contains("Opção inválida", texto);
        Assert.Contains("Número inválido", texto);
        Assert.Contains("Saldo em BRL insuficiente", texto);
        Assert.Contains("Saldo em BTC insuficiente", texto);
        Assert.Contains("Nenhuma transação", texto);
    }

    [Theory]
    [InlineData("")]
    [InlineData("3\n")]
    public void FimDaEntradaEncerraSemLoop(string texto)
    {
        using var entrada = new StringReader(texto);
        using var saida = new StringWriter();
        new AplicacaoConsole(entrada, saida).Executar();
        Assert.Contains("Até logo", saida.ToString());
    }
}
