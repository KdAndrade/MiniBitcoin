# MiniBitcoin

Aplicação de console em C# para simular uma carteira de Bitcoin. Projeto da atividade de Garantia da Qualidade de Software / Gestão e Qualidade de Software, com testes xUnit, desenvolvimento orientado a testes e Conventional Commits.

## Funcionalidades

- Consultar saldos em reais, Bitcoin e satoshis.
- Consultar e atualizar a cotação simulada do Bitcoin.
- Comprar e vender frações de Bitcoin, validando os fundos disponíveis.
- Exibir o extrato cronológico de compras e vendas, com data, quantidade, cotação e total em reais.
- Calcular o patrimônio consolidado: saldo em BRL + saldo em BTC × cotação atual.
- Recusar valores negativos, precisão inválida, operações sem saldo e totais que arredondem para zero.

## Pré-requisitos e execução

Instale o .NET SDK 10.0 (o projeto usa `net10.0`) e o Git. A primeira restauração exige acesso à internet para baixar os pacotes NuGet. Não é necessário banco de dados ou chave de API.

```sh
git clone https://github.com/KdAndrade/MiniBitcoin.git
cd MiniBitcoin
dotnet restore
dotnet build
dotnet run --project MiniBitcoin.Console/MiniBitcoin.Console.csproj
```

## Testes

```sh
dotnet test
dotnet test -c Release
```

São **35 casos de teste**, incluindo dados parametrizados. A suíte cobre carteira, negociação, extrato, patrimônio e interação com o menu. Os testes usam valores simulados e não dependem de serviços externos.

## Uso do menu

Cada execução começa com **R$ 1.000,00**, **0 BTC** e cotação de **R$ 100.000,00 por BTC**.

| Opção | Ação |
| --- | --- |
| 1 | Consultar cotação |
| 2 | Consultar saldos e patrimônio |
| 3 | Comprar, informando a quantidade de BTC |
| 4 | Vender, informando a quantidade de BTC |
| 5 | Exibir extrato e patrimônio |
| 6 | Atualizar a cotação em BRL por BTC |
| 0 | Encerrar |

Digite números **sem separador de milhar**. São aceitos ponto ou vírgula como separador decimal, por exemplo `0,005` ou `0.005` BTC. Para uma cotação de duzentos mil reais, digite `200000`.

### Exemplo

1. Escolher `3` e comprar `0,005` BTC: saldo de R$ 500,00 e 0,005 BTC.
2. Escolher `6` e informar `200000`: cotação de R$ 200.000,00 por BTC.
3. Escolher `4` e vender `0,002` BTC: saldo de R$ 900,00 e 0,003 BTC.
4. Escolher `5`: extrato com compra de R$ 500,00 e venda de R$ 400,00; patrimônio atual de R$ 1.500,00.
5. Escolher `0` para sair.

## Arquitetura

```text
MiniBitcoin.sln
MiniBitcoin.Console/
  Program.cs                       Ponto de entrada
  Dominio/
    Carteira.cs                    Saldos e validação de créditos/débitos
    Mercado.cs                     Cotação, negociação, histórico e patrimônio
    Transacao.cs                   Registro imutável de compra ou venda
  Interface/
    AplicacaoConsole.cs            Menu, leitura e apresentação em português
MiniBitcoin.Tests/
  CarteiraTests.cs                 Validação de saldos e débitos
  MercadoTests.cs                  Compra, venda, cotação e arredondamento
  ExtratoTests.cs                  Histórico e patrimônio
  ConsoleTests.cs                  Fluxos do menu e tratamento de entrada
```

As regras de negócio ficam separadas da interface. A carteira mantém setters privados; o mercado coordena as trocas entre os saldos e só registra operações concluídas. O histórico é exposto como coleção somente leitura e cada transação preserva a cotação utilizada. O menu recebe `TextReader` e `TextWriter`, permitindo testar a interação sem usar o terminal real.

## Regras e precisão

- Valores monetários e quantidades são representados por `decimal`.
- Saldos iniciais podem ser zero, mas não negativos.
- BRL e cotação aceitam até duas casas decimais; BTC aceita até oito. Um BTC corresponde a 100.000.000 satoshis.
- Compra e venda recebem a quantidade de BTC e calculam o total em BRL pela cotação vigente.
- O total é arredondado para centavos com `MidpointRounding.AwayFromZero`: R$ 0,015 resulta em R$ 0,02. Operações cujo total resulte em zero são recusadas.
- Compra exige BRL suficiente; venda exige BTC suficiente. O crédito é verificado antes do débito para evitar alterações parciais por overflow.
- Mudar a cotação altera a avaliação atual da carteira, preservando os registros anteriores.
- As datas são armazenadas em UTC e exibidas no fuso local do computador.

## Desenvolvimento e histórico Git

O desenvolvimento foi dividido nas etapas da atividade. Em cada funcionalidade, os testes foram escritos e executados antes da implementação; a primeira execução falhou pela ausência dos tipos ou membros esperados. Depois da implementação, a suíte correspondente foi executada novamente até passar.

Os commits separam estrutura inicial (`chore`), testes (`test`), funcionalidades (`feat`), refatoração (`refactor`) e documentação (`docs`). A refatoração centralizou o arredondamento e o registro das transações.

```sh
git log --oneline
git status
```

O commit `Initial commit`, que já existia no GitHub antes da atividade, foi preservado. Todos os novos commits seguem Conventional Commits.

## Limitações da simulação

Os dados ficam em memória e são reiniciados ao encerrar o aplicativo. A cotação é simulada e atualizada manualmente; não há conexão com corretoras, blockchain, taxas, autenticação ou movimentação de dinheiro real.

## Licença

MIT, conforme o arquivo [LICENSE](LICENSE).
