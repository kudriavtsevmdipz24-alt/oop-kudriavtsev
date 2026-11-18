using System;
using System.Net.Http;
using System.Threading;

namespace IndependentWork20
{
    public class RetryPolicy
    {
        private readonly int _maxRetries;
        private readonly TimeSpan _initialDelay;
        private readonly double _jitterFactor;
        private readonly Random _random = new();

        public RetryPolicy(int maxRetries, TimeSpan initialDelay, double jitterFactor = 0.2)
        {
            if (maxRetries < 0)
                throw new ArgumentOutOfRangeException(nameof(maxRetries), "Кількість повторів не може бути від'ємною.");
            if (initialDelay < TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(initialDelay), "Початкова затримка не може бути від'ємною.");
            if (jitterFactor < 0 || jitterFactor > 1)
                throw new ArgumentOutOfRangeException(nameof(jitterFactor), "Коефіцієнт jitter має бути в межах [0; 1].");

            _maxRetries = maxRetries;
            _initialDelay = initialDelay;
            _jitterFactor = jitterFactor;
        }

        public T Execute<T>(Func<T> operation, Func<Exception, bool>? shouldRetry = null)
        {
            ArgumentNullException.ThrowIfNull(operation);
            shouldRetry ??= _ => true; // За замовчуванням повторюємо для будь-якого винятку

            int retryAttempt = 0;

            while (true)
            {
                try
                {
                    return operation();
                }
                catch (Exception ex) when (retryAttempt < _maxRetries && shouldRetry(ex))
                {
                    // Обов'язкова формула exponential backoff з jitter:
                    double multiplier = Math.Pow(2, retryAttempt) * (1 + _random.NextDouble() * _jitterFactor * 2 - _jitterFactor);
                    TimeSpan delay = TimeSpan.FromMilliseconds(_initialDelay.TotalMilliseconds * Math.Max(0, multiplier));

                    retryAttempt++;
                    Console.WriteLine($"  [Retry #{retryAttempt}/{_maxRetries}] Помилка ({ex.GetType().Name}): {ex.Message} -> Очікування {delay.TotalMilliseconds:F0} мс...");
                    Thread.Sleep(delay);
                }
            }
        }
    }

    class Program
    {
        static void Main()
        {
            Console.WriteLine("=== Самостійна робота №20: Реалізація Exponential Backoff з Jitter ===\n");

            var policy = new RetryPolicy(maxRetries: 3, initialDelay: TimeSpan.FromMilliseconds(200), jitterFactor: 0.25);

            // -----------------------------------------------------------------
            // Сценарій 1: Успішне виконання після 2 тимчасових невдач
            // -----------------------------------------------------------------
            Console.WriteLine("--- 1. Операція з тимчасовими збоями (успіх на 3-й спробі) ---");
            int attempt1 = 0;
            try
            {
                string response = policy.Execute(
                    operation: () =>
                    {
                        attempt1++;
                        if (attempt1 <= 2)
                            throw new HttpRequestException($"Сервер тимчасово перевантажений (503 Service Unavailable, спроба {attempt1})");
                        return "Дані профілю успішно завантажено (200 OK)";
                    },
                    shouldRetry: ex => ex is HttpRequestException
                );
                Console.WriteLine($"[Успіх] Результат: {response}\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Критична помилка] {ex.Message}\n");
            }

            // -----------------------------------------------------------------
            // Сценарій 2: Постійний збій -> вичерпання всіх спроб та викидання винятку
            // -----------------------------------------------------------------
            Console.WriteLine("--- 2. Операція з постійним збоєм (вичерпання maxRetries) ---");
            int attempt2 = 0;
            try
            {
                policy.Execute<int>(
                    operation: () =>
                    {
                        attempt2++;
                        throw new TimeoutException($"База даних не відповідає (спроба {attempt2})");
                    },
                    shouldRetry: ex => ex is TimeoutException
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Остаточна невдача після {attempt2} спроб] Перехоплено виняток: {ex.GetType().Name} — {ex.Message}\n");
            }

            // -----------------------------------------------------------------
            // Сценарій 3: Фільтрація через shouldRetry (неповторювана помилка)
            // -----------------------------------------------------------------
            Console.WriteLine("--- 3. Помилка валідації (shouldRetry повертає false — без повторів) ---");
            try
            {
                policy.Execute<string>(
                    operation: () => throw new ArgumentException("Некоректний формат ID користувача"),
                    shouldRetry: ex => ex is HttpRequestException || ex is TimeoutException
                );
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"[Миттєвий вихід без Retry] {ex.GetType().Name}: {ex.Message}");
            }
        }
    }
}
