using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

namespace IsPrime
{
    internal class Program
    {
        private static BigInteger bigNumber = BigInteger.Zero;

        private static void Main()
        {
            while (true)
            {
                while (bigNumber.IsZero)
                {
                    Console.Write("Inserisci numero di partenza: ");
                    var start = Console.ReadLine();

                    if (BigInteger.TryParse(start, out var parsed))
                    {
                        if (parsed < 0)
                        {
                            Console.WriteLine("Richiesto un numero positivo!");
                            continue;
                        }

                        bigNumber = parsed;
                    }
                    else
                    {
                        Console.WriteLine("???");
                        continue;
                    }
                }

                if (IsPrime(bigNumber))
                {
                    Console.WriteLine($"Found prime: {bigNumber}");
                }
                else
                {
                    Console.WriteLine($"NOT a prime number: {bigNumber}");
                }

                //Parallel.ForEach(Infinite(), n => IsPrime(n));

                bigNumber = BigInteger.Zero;
            }
        }

        private static bool IsPrime(BigInteger number)
        {
            if (number < 0)
                throw new ArgumentException("Negative number is not supported");
            if (number <= 3)
                return true;
            if (number.IsEven)
                return false;

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var max = number / 2;

            /*
            BigInteger i = 3;
            for (; i < max; i++)
            {
                if (number % i == 0)
                {
                    return false;
                }
            }
            */

            var cancellationTokenSource = new CancellationTokenSource();
            var tasks = new Task<bool>[8];
            var chuck = number / tasks.Length;

            for (int t = 0; t < tasks.Length; t++)
            {
                var from = BigInteger.Max(3, chuck * t);
                var to = chuck * (t + 1);

                var task = Task.Run(() => Fuck(from, to, number, cancellationTokenSource));
                tasks[t] = task;
            }

            Task.WaitAll(tasks);

            stopwatch.Stop();
            TimeSpan ts = stopwatch.Elapsed;
            Console.WriteLine($"Tempo di calcolo: {ts.Hours}:{ts.Minutes}:{ts.Seconds}.{ts.Milliseconds}");

            return tasks.All((t) => t.Result);
        }

        private static bool Fuck(BigInteger from, BigInteger to, BigInteger number, CancellationTokenSource cancellationTokenSource)
        {
            for (; from < to; from++)
            {
                if (number.IsEven || number % from == 0 || cancellationTokenSource.IsCancellationRequested)
                {
                    cancellationTokenSource.Cancel();
                    return false;
                }
            }

            return true;
        }

        private static IEnumerable<BigInteger> Infinite()
        {
            while (true) yield return ++bigNumber;
        }
    }
}