using System;
using System.Collections.Generic;
using System.Globalization;

class Program
{
    static void Main()
    {
        bool repetir;

        do
        {
            List<Investimento> investimentos = new List<Investimento>();

            Console.WriteLine("Simulador de Rendimento Bancário \n");
            Console.WriteLine("Vamos fazer uma simulação do seu investimento bancário? \n");

            Console.Write("Quantos depósitos você deseja simular? ");
            int quantidade = int.Parse(Console.ReadLine());

            for (int i = 0; i < quantidade; i++)
            {
                Console.WriteLine($"\n--- Depósito #{i + 1} ---");

                decimal valor;
                while (true)
                {
                    Console.Write("Digite o valor do depósito (R$): ");
                    if (decimal.TryParse(Console.ReadLine(), out valor) && valor > 0)
                        break;
                    Console.WriteLine("Valor inválido. Tente novamente.");
                }

                decimal taxa;
                while (true)
                {
                    Console.Write("Digite a taxa mensal de rendimento (%): ");
                    if (decimal.TryParse(Console.ReadLine(), out taxa) && taxa >= 0)
                    {
                        taxa /= 100;
                        break;
                    }
                    Console.WriteLine("Taxa inválida. Tente novamente.");
                }

                int meses;
                while (true)
                {
                    Console.Write("Digite o número de meses: ");
                    if (int.TryParse(Console.ReadLine(), out meses) && meses >= 0)
                        break;
                    Console.WriteLine("Número de meses inválido.");
                }

                int dias;
                while (true)
                {
                    Console.Write("Digite o número de dias adicionais: ");
                    if (int.TryParse(Console.ReadLine(), out dias) && dias >= 0)
                        break;
                    Console.WriteLine("Número de dias inválido.");
                }

                var investimento = new Investimento(valor, taxa, meses, dias);
                investimentos.Add(investimento);
            }

            Console.Clear();
            Console.WriteLine("Resultados da Simulação:\n");

            int count = 1;
            foreach (var inv in investimentos)
            {
                Console.WriteLine($"=== Depósito #{count} ===");
                inv.ExibirTabelaComData();
                inv.SimularSaque();
                count++;
            }

            Console.Write("Deseja fazer uma nova simulação? (s/n): ");
            string resposta = Console.ReadLine().ToLower();
            repetir = resposta == "s" || resposta == "sim";

        } while (repetir);
    }
}

class Investimento
{
    public decimal ValorInicial { get; }
    public decimal TaxaMensal { get; }
    public int Meses { get; }
    public int Dias { get; }
    public DateTime DataInicio { get; }

    public Investimento(decimal valor, decimal taxaMensal, int meses, int dias)
    {
        ValorInicial = valor;
        TaxaMensal = taxaMensal;
        Meses = meses;
        Dias = dias;
        DataInicio = DateTime.Today;
    }

    public void ExibirTabelaComData()
    {
        MostrarSimulacao(ValorInicial, null, null);
    }

    public void SimularSaque()
    {
        Console.Write("Deseja realizar um saque antes do fim do investimento? (s/n): ");
        string resposta = Console.ReadLine().ToLower();

        if (resposta != "s" && resposta != "sim")
        {
            Console.WriteLine("Nenhum saque realizado.\n");
            return;
        }

        int mesSaque = 0;
        decimal valorSaque = 0;

        while (true)
        {
            Console.Write($"Em qual mês deseja realizar o saque (1 a {Meses}): ");
            if (int.TryParse(Console.ReadLine(), out mesSaque) && mesSaque >= 1 && mesSaque <= Meses)
                break;
            Console.WriteLine("Mês inválido. Tente novamente.");
        }

        while (true)
        {
            Console.Write("Qual valor deseja sacar (R$): ");
            if (decimal.TryParse(Console.ReadLine(), out valorSaque) && valorSaque > 0)
                break;
            Console.WriteLine("Valor inválido. Tente novamente.");
        }

        Console.WriteLine("\n=== Simulação após Saque ===");
        MostrarSimulacao(ValorInicial, mesSaque, valorSaque);
    }

    private void MostrarSimulacao(decimal valorInicial, int? mesSaque, decimal? valorSaque)
    {
        decimal saldo = valorInicial;
        decimal rendimentoTotal = 0;
        DateTime data = DataInicio;
        CultureInfo br = new CultureInfo("pt-BR");

        Console.WriteLine("Data       | Saldo (R$)   | Rend. Mês (R$) | Rend. Líquido (R$)");
        Console.WriteLine("---------------------------------------------------------------");
        Console.WriteLine($"{data:dd/MM/yyyy} | {saldo,12:F2} | {0,13:F2} | {0,17:F2}");

        for (int i = 1; i <= Meses; i++)
        {
            decimal saldoAnterior = saldo;
            decimal rendimentoMensal = saldo * TaxaMensal;
            saldo += rendimentoMensal;
            rendimentoTotal += rendimentoMensal;
            data = data.AddMonths(1);

            if (mesSaque.HasValue && i == mesSaque.Value)
            {
                if (saldo >= valorSaque.Value)
                {
                    saldo -= valorSaque.Value;
                    Console.WriteLine($">>> Saque de R$ {valorSaque:F2} realizado em {data:dd/MM/yyyy} <<<");
                }
                else
                {
                    Console.WriteLine($">>> Saque NÃO realizado em {data:dd/MM/yyyy}: saldo insuficiente <<<");
                }
            }

            Console.WriteLine($"{data:dd/MM/yyyy} | {saldo,12:F2} | {rendimentoMensal,13:F2} | {rendimentoTotal,17:F2}");
        }

        if (Dias > 0)
        {
            decimal taxaDiaria = TaxaMensal / 30;
            decimal saldoAntes = saldo;

            for (int i = 0; i < Dias; i++)
            {
                decimal rendimentoDiario = saldo * taxaDiaria;
                saldo += rendimentoDiario;
                rendimentoTotal += rendimentoDiario;
            }

            data = data.AddDays(Dias);
            decimal rendimentoDias = saldo - saldoAntes;

            Console.WriteLine($"{data:dd/MM/yyyy} | {saldo,12:F2} | {rendimentoDias,13:F2} | {rendimentoTotal,17:F2}");
        }

        Console.WriteLine("---------------------------------------------------------------");
        Console.WriteLine($"Valor Inicial: R$ {valorInicial:F2}");
        Console.WriteLine($"Valor Final:   R$ {saldo:F2}");
        Console.WriteLine($"Rendimento Líquido Total: R$ {rendimentoTotal:F2}");
        Console.WriteLine("---------------------------------------------------------------\n");
    }
}
