using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace IndependentWork18
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("=== Самостійна робота №18: PLINQ — продуктивність та потокобезпечність ===\n");

            // 1. Порівняння продуктивності LINQ vs PLINQ (1M, 5M, 10M елементів)
            Console.WriteLine("--- Частина 1: Бенчмарк LINQ vs PLINQ (пошук простих чисел) ---");
            foreach (int size in new[] { 1_000_000, 5_000_000, 10_000_000 })
            {
                RunPerformanceTest(size);
            }

            // 2. Дослідження потокобезпечності (побічні ефекти в PLINQ)
            Console.WriteLine("\n--- Частина 2: Проблема безпеки (Race Condition у PLINQ) ---");
            RunThreadSafetyDemo();
        }

        // Обчислювально інтенсивна операція: перевірка на просте число
        static bool IsPrime(int n)
        {
            if (n < 2) return false;
            if (n == 2) return true;
            if (n % 2 == 0) return false;

            int limit = (int)Math.Sqrt(n);
            for (int i = 3; i <= limit; i += 2)
            {
                if (n % i == 0) return false;
            }
            return true;
        }

        static void RunPerformanceTest(int count)
        {
            var rnd = new Random(42);
            var data = Enumerable.Range(0, count).Select(_ => rnd.Next(10_000, 500_000)).ToList();

            // Звичайний послідовний LINQ
            var sw = Stopwatch.StartNew();
            var linqRes = data.Where(IsPrime).Select(x => Math.Sqrt(x)).ToList();
            sw.Stop();
            long linqMs = sw.ElapsedMilliseconds;

            // Паралельний PLINQ
            sw.Restart();
            var plinqRes = data.AsParallel().Where(IsPrime).Select(x => Math.Sqrt(x)).ToList();
            sw.Stop();
            long plinqMs = sw.ElapsedMilliseconds;

            double speedup = plinqMs > 0 ? (double)linqMs / plinqMs : 0;
            Console.WriteLine($"Елементів: {count,10:N0} | LINQ: {linqMs,5} мс | PLINQ: {plinqMs,5} мс | Прискорення: {speedup:F2}x (Знайдено: {linqRes.Count})");
        }

        static void RunThreadSafetyDemo()
        {
            var numbers = Enumerable.Range(1, 1_000_000).ToList();
            int expected = numbers.Count(x => x % 2 == 0);

            // 1. Небезпечний підхід (модифікація спільної змінної без синхронізації)
            int unsafeCounter = 0;
            numbers.AsParallel().ForAll(n =>
            {
                if (n % 2 == 0)
                    unsafeCounter++; // Неатомарна операція (Race Condition!)
            });

            // 2. Потокобезпечний підхід через Interlocked.Increment
            int safeInterlockedCounter = 0;
            numbers.AsParallel().ForAll(n =>
            {
                if (n % 2 == 0)
                    Interlocked.Increment(ref safeInterlockedCounter);
            });

            // 3. Потокобезпечний підхід через lock
            int safeLockCounter = 0;
            object locker = new();
            numbers.AsParallel().ForAll(n =>
            {
                if (n % 2 == 0)
                {
                    lock (locker)
                    {
                        safeLockCounter++;
                    }
                }
            });

            Console.WriteLine($"Очікувана кількість парних чисел : {expected:N0}");
            Console.WriteLine($"Без синхронізації (unsafeCounter) : {unsafeCounter:N0}  <-- ВТРАТА ДАНИХ!");
            Console.WriteLine($"З Interlocked.Increment           : {safeInterlockedCounter:N0}  <-- Коректно");
            Console.WriteLine($"З блокуванням lock                : {safeLockCounter:N0}  <-- Коректно");
        }
    }
}