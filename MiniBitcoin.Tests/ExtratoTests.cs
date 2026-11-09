using MiniBitcoin.Dominio;

namespace MiniBitcoin.Tests;

public class ExtratoTests
{
    [Fact]
    public void IniciaSemTransacoes()
    {
        var mercado = new Mercado(new Carteira(100), 100000);
        Assert.Empty(mercado.Historico);
        Assert.Equal(100, mercado.PatrimonioTotal);
    }

    [Fact]
    public void RegistraOperacoesEmOrdemComCotacaoEDadosOriginais()
    {
        var mercado = new Mercado(new Carteira(1000), 100000);
        var antes = DateTimeOffset.UtcNow;
        mercado.Comprar(0.005m);
        mercado.AtualizarCotacao(200000);
        mercado.Vender(0.002m);
        Assert.Collection(mercado.Historico,
            compra =>
            {
                Assert.Equal(TipoTransacao.Compra, compra.Tipo);
                Assert.Equal(0.005m, compra.QuantidadeBtc);
                Assert.Equal(100000, compra.Cotacao);
                Assert.Equal(500, compra.TotalBrl);
                Assert.InRange(compra.Data, antes, DateTimeOffset.UtcNow);
            },
            venda =>
            {
                Assert.Equal(TipoTransacao.Venda, venda.Tipo);
                Assert.Equal(0.002m, venda.QuantidadeBtc);
                Assert.Equal(200000, venda.Cotacao);
                Assert.Equal(400, venda.TotalBrl);
            });
        Assert.Equal(1500, mercado.PatrimonioTotal);
    }

    [Fact]
    public void OperacoesRecusadasNaoEntramNoExtrato()
    {
        var mercado = new Mercado(new Carteira(), 100000);
        Assert.Throws<ArgumentException>(() => mercado.Comprar(1));
        Assert.Throws<ArgumentException>(() => mercado.Vender(1));
        Assert.Empty(mercado.Historico);
    }

    [Fact]
    public void CotacaoAtualReavaliaPatrimonioSemAlterarHistorico()
    {
        var mercado = new Mercado(new Carteira(100, 0.01m), 100000);
        Assert.Equal(1100, mercado.PatrimonioTotal);
        mercado.AtualizarCotacao(200000);
        Assert.Equal(2100, mercado.PatrimonioTotal);
        Assert.Empty(mercado.Historico);
    }

    [Fact]
    public void HistoricoNaoPermiteAlteracaoExterna()
    {
        var mercado = new Mercado(new Carteira(100000), 100000);
        mercado.Comprar(1);
        var lista = Assert.IsAssignableFrom<IList<Transacao>>(mercado.Historico);
        Assert.Throws<NotSupportedException>(() => lista.Clear());
        Assert.Single(mercado.Historico);
    }
}
