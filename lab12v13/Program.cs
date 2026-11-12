using System;
using System.IO;
using System.Net.Http;
using System.Threading;

namespace lab12v13
{
    // 1. Узагальнений клас для реалізації патерну Retry з експоненційною затримкою
    public static class RetryHelper
    {
        public static T ExecuteWithRetry<T>(
            Func<T> operation,
            int retryCount = 3,
            TimeSpan initialDelay = default,
            Func<Exception, bool> shouldRetry = null)
        {
            if (operation == null)
                throw new ArgumentNullException(nameof(operation));

            if (initialDelay == default)
                initialDelay = TimeSpan.FromMilliseconds(200);

            int attempt = 0;

            while (true)
            {
                try
                {
                    attempt++;
                    Console.WriteLine($"[RetryHelper] Спроба #{attempt}...");
                    T result = operation();
                    Console.WriteLine($"[RetryHelper] Спроба #{attempt} успішна!");
                    return result;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[RetryHelper] Помилка на спробі #{attempt}: {ex.GetType().Name} — {ex.Message}");

                    // Перевіряємо, чи дозволено повторювати для цього типу винятку
                    bool canRetry = shouldRetry == null || shouldRetry(ex);

                    if (!canRetry)
                    {
                        Console.WriteLine($"[RetryHelper] Виняток {ex.GetType().Name} не підлягає повтору (shouldRetry = false). Негайне переривання.");
                        throw;
                    }

                    if (attempt > retryCount)
                    {
                        Console.WriteLine($"[RetryHelper] Вичерпано ліміт повторних спроб ({retryCount}). Операцію провалено.");
                        throw;
                    }

                    // Експоненційна затримка: initialDelay * 2^(attempt - 1)
                    double multiplier = Math.Pow(2, attempt - 1);
                    TimeSpan delay = TimeSpan.FromMilliseconds(initialDelay.TotalMilliseconds * multiplier);

                    Console.WriteLine($"[RetryHelper] Очікування {delay.TotalMilliseconds} мс перед наступною спробою...\n");
                    Thread.Sleep(delay);
                }
            }
        }
    }

    // 2. Клас FileProcessor (Варіант 13: імітація IOException перші 2 рази)
    public class FileProcessor
    {
        private int _callCount = 0;

        public double ReadSensorData(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Шлях до файлу не може бути порожнім.", nameof(path));

            _callCount++;
            if (_callCount <= 2)
            {
                throw new IOException($"Файл '{path}' тимчасово заблокований іншим процесом (збій #{_callCount}).");
            }

            return 23.5; // Температура з локального логу сенсора
        }
    }

    // 3. Клас NetworkClient (Варіант 13: імітація HttpRequestException перші 4 рази)
    public class NetworkClient
    {
        private int _requestCount = 0;

        public double GetRemoteSensorData(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("URL не може бути порожнім.", nameof(url));

            _requestCount++;
            if (_requestCount <= 4)
            {
                throw new HttpRequestException($"Таймаут підключення до віддаленого сенсора за адресою '{url}' (збій #{_requestCount}).");
            }

            return 24.1; // Показник віддаленого сенсора
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Лабораторна робота №12: Обробка IO/мережевих помилок та патерн Retry ===");
            Console.WriteLine("=== Варіант 13: Читання даних з сенсора ===\n");

            // Делегат вибіркової перевірки винятків згідно з варіантом 13
            Func<Exception, bool> retryPolicy = ex =>
                ex is IOException || ex is HttpRequestException;

            // -----------------------------------------------------------------
            // Сценарій 1: Читання локального файлу сенсора (2 збої, 3-тя спроба — успіх)
            // -----------------------------------------------------------------
            Console.WriteLine("--- Сценарій 1: FileProcessor.ReadSensorData (очікується 2 збої IOException) ---");
            var fileProcessor = new FileProcessor();

            try
            {
                double localTemp = RetryHelper.ExecuteWithRetry(
                    operation: () => fileProcessor.ReadSensorData("C:\\sensors\\temp_log.dat"),
                    retryCount: 3,
                    initialDelay: TimeSpan.FromMilliseconds(200),
                    shouldRetry: retryPolicy
                );
                Console.WriteLine($">>> Результат локального сенсора: {localTemp} °C\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($">>> Фатальна помилка: {ex.Message}\n");
            }

            // -----------------------------------------------------------------
            // Сценарій 2: Запит до віддаленого сенсора (4 збої, 5-та спроба — успіх)
            // -----------------------------------------------------------------
            Console.WriteLine("--- Сценарій 2: NetworkClient.GetRemoteSensorData (очікується 4 збої HttpRequestException) ---");
            var networkClient = new NetworkClient();

            try
            {
                // Передаємо retryCount = 4, щоб сумарно було 5 спроб (1 основна + 4 повторні)
                double remoteTemp = RetryHelper.ExecuteWithRetry(
                    operation: () => networkClient.GetRemoteSensorData("https://api.iot-sensors.local/v1/temp"),
                    retryCount: 4,
                    initialDelay: TimeSpan.FromMilliseconds(150),
                    shouldRetry: retryPolicy
                );
                Console.WriteLine($">>> Результат віддаленого сенсора: {remoteTemp} °C\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($">>> Фатальна помилка: {ex.Message}\n");
            }

            // -----------------------------------------------------------------
            // Сценарій 3: Демонстрація роботи shouldRetry (неповторюваний виняток)
            // -----------------------------------------------------------------
            Console.WriteLine("--- Сценарій 3: Перевірка shouldRetry при некоректному аргументі (ArgumentException) ---");
            try
            {
                RetryHelper.ExecuteWithRetry(
                    operation: () => fileProcessor.ReadSensorData(""),
                    retryCount: 3,
                    initialDelay: TimeSpan.FromMilliseconds(100),
                    shouldRetry: retryPolicy
                );
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($">>> Очікувано перехоплено без повторних спроб: {ex.Message}\n");
            }
        }
    }
}