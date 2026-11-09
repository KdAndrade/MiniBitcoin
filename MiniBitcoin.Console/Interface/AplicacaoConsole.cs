using System.Globalization;
using MiniBitcoin.Dominio;

namespace MiniBitcoin.Interface;

public sealed class AplicacaoConsole(TextReader entrada, TextWriter saida)
{
    private static readonly CultureInfo Cultura = CultureInfo.GetCultureInfo("pt-BR");
    private readonly Carteira carteira = new(1000);

    public void Executar()
    {
        var mercado = new Mercado(carteira, 100000);
        saida.WriteLine("MiniBitcoin - simulação de carteira");
        saida.WriteLine("Saldo inicial: R$ 1.000,00 e 0 BTC. Cotação inicial: R$ 100.000,00.");
        saida.WriteLine("Informe números sem separador de milhar; use vírgula ou ponto decimal.");
        while (true)
        {
            saida.WriteLine("\n1 - Cotação | 2 - Saldos | 3 - Comprar BTC | 4 - Vender BTC | 5 - Extrato | 6 - Atualizar cotação | 0 - Sair");
            saida.Write("Opção: ");
            var opcao = entrada.ReadLine();
            if (opcao is null || opcao.Trim() == "0") break;
            try
            {
                switch (opcao.Trim())
                {
                    case "1":
                        saida.WriteLine($"Cotação simulada: {Moeda(mercado.Cotacao)} por BTC");
                        break;
                    case "2":
                        ExibirSaldos(mercado);
                        break;
                    case "3":
                        mercado.Comprar(LerNumero("Quantidade de BTC para comprar (até 8 casas): "));
                        saida.WriteLine("Compra realizada com sucesso.");
                        break;
                    case "4":
                        mercado.Vender(LerNumero("Quantidade de BTC para vender (até 8 casas): "));
                        saida.WriteLine("Venda realizada com sucesso.");
                        break;
                    case "5":
                        if (mercado.Historico.Count == 0) saida.WriteLine("Nenhuma transação realizada.");
                        foreach (var transacao in mercado.Historico)
                            saida.WriteLine($"{transacao.Data.ToLocalTime():dd/MM/yyyy HH:mm:ss zzz} | {transacao.Tipo} | {transacao.QuantidadeBtc.ToString("F8", Cultura)} BTC | Cotação: {Moeda(transacao.Cotacao)} | Total: {Moeda(transacao.TotalBrl)}");
                        ExibirSaldos(mercado);
                        break;
                    case "6":
                        mercado.AtualizarCotacao(LerNumero("Nova cotação em BRL por BTC: "));
                        saida.WriteLine("Cotação atualizada.");
                        break;
                    default:
                        saida.WriteLine("Opção inválida.");
                        break;
                }
            }
            catch (EndOfStreamException) { break; }
            catch (ArgumentException erro) { saida.WriteLine($"Operação recusada: {erro.Message}"); }
            catch (FormatException erro) { saida.WriteLine(erro.Message); }
            catch (OverflowException) { saida.WriteLine("Valor fora do limite suportado."); }
        }
        saida.WriteLine("Até logo!");
    }

    private decimal LerNumero(string mensagem)
    {
        saida.Write(mensagem);
        var texto = entrada.ReadLine() ?? throw new EndOfStreamException();
        if (!decimal.TryParse(texto.Trim().Replace(',', '.'),
            NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint,
            CultureInfo.InvariantCulture, out var valor))
            throw new FormatException("Número inválido. Informe um número sem separador de milhar.");
        return valor;
    }

    private void ExibirSaldos(Mercado mercado)
    {
        saida.WriteLine($"Saldo em BRL: {Moeda(carteira.SaldoBrl)}");
        saida.WriteLine($"Saldo em Bitcoin: {carteira.SaldoBtc.ToString("F8", Cultura)} BTC ({carteira.SaldoSatoshis.ToString("N0", Cultura)} satoshis)");
        saida.WriteLine($"Patrimônio total: {Moeda(mercado.PatrimonioTotal)}");
    }

    private static string Moeda(decimal valor) => valor.ToString("C2", Cultura);
}
