using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Polly;
using Polly.CircuitBreaker;
using Polly.Fallback;
using Polly.Retry;
using Polly.Timeout;

namespace IndependentWork19
{
    // 1. Власний тип винятку з трьома стандартними конструкторами та контекстними властивостями
    public class PaymentGatewayTimeoutException : Exception
    {
        public string TransactionId { get; }
        public string GatewayName { get; }

        public PaymentGatewayTimeoutException() { }

        public PaymentGatewayTimeoutException(string message) : base(message) { }

        public PaymentGatewayTimeoutException(string message, Exception innerException)
            : base(message, innerException) { }

        public PaymentGatewayTimeoutException(string message, string transactionId, string gatewayName)
            : base(message)
        {
            TransactionId = transactionId;
            GatewayName = gatewayName;
        }
    }

    class Program
    {
        static async Task Main()
        {
            Console.WriteLine("=== Самостійна робота №19: Дизайн винятків і політики Polly ===\n");

            DemoCustomException();
            DemoPollyRetryScenario();
            await DemoPollyCircuitBreakerScenario();
            await DemoPollyTimeoutWithFallbackScenario();
        }

        // Демонстрація власного винятку
        static void DemoCustomException()
        {
            Console.WriteLine("--- 1. Демонстрація власного винятку PaymentGatewayTimeoutException ---");
            try
            {
                throw new PaymentGatewayTimeoutException(
                    "Платіжний шлюз не відповів вчасно.",
                    transactionId: "TXN-98451",
                    gatewayName: "LiqPay-API");
            }
            catch (PaymentGatewayTimeoutException ex)
            {
                Console.WriteLine($"[Catch] Помилка шлюзу '{ex.GatewayName}' (Транзакція: {ex.TransactionId}): {ex.Message}\n");
            }
        }

        // Сценарій 1 Polly: Retry з експоненційною затримкою та Jitter (виклик API оплати)
        static void DemoPollyRetryScenario()
        {
            Console.WriteLine("--- 2. Сценарій Polly #1: Retry + Exponential Backoff + Jitter ---");
            int attempt = 0;

            var retryPipeline = new ResiliencePipelineBuilder()
                .AddRetry(new RetryStrategyOptions
                {
                    ShouldHandle = new PredicateBuilder().Handle<PaymentGatewayTimeoutException>(),
                    MaxRetryAttempts = 3,
                    Delay = TimeSpan.FromMilliseconds(200),
                    BackoffType = DelayBackoffType.Exponential,
                    UseJitter = true,
                    OnRetry = args =>
                    {
                        Console.WriteLine($"  [Polly Retry] Спроба #{args.AttemptNumber + 1} після затримки {args.RetryDelay.TotalMilliseconds:F0} мс. Причина: {args.Outcome.Exception?.Message}");
                        return default;
                    }
                })
                .Build();

            try
            {
                string result = retryPipeline.Execute(() =>
                {
                    attempt++;
                    if (attempt <= 2)
                        throw new PaymentGatewayTimeoutException($"Таймаут з'єднання (збій #{attempt})", "TXN-101", "Stripe");
                    return "Оплата підтверджена (200 OK)";
                });
                Console.WriteLine($"[Результат] {result}\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Остаточна невдача] {ex.Message}\n");
            }
        }

        // Сценарій 2 Polly: Circuit Breaker (захист від «падіння» БД)
        static async Task DemoPollyCircuitBreakerScenario()
        {
            Console.WriteLine("--- 3. Сценарій Polly #2: Circuit Breaker (Автоматичний вимикач для БД) ---");

            var breakerPipeline = new ResiliencePipelineBuilder()
                .AddCircuitBreaker(new CircuitBreakerStrategyOptions
                {
                    ShouldHandle = new PredicateBuilder().Handle<InvalidOperationException>(),
                    FailureRatio = 0.5,
                    MinimumThroughput = 2,
                    SamplingDuration = TimeSpan.FromSeconds(10),
                    BreakDuration = TimeSpan.FromMilliseconds(600),
                    OnOpened = args =>
                    {
                        Console.WriteLine("  [Circuit Breaker] Стан -> OPEN! Виклики до БД заблоковано на 600 мс.");
                        return default;
                    },
                    OnHalfOpened = _ =>
                    {
                        Console.WriteLine("  [Circuit Breaker] Стан -> HALF-OPEN! Пробний запит до БД...");
                        return default;
                    },
                    OnClosed = _ =>
                    {
                        Console.WriteLine("  [Circuit Breaker] Стан -> CLOSED! Роботу БД відновлено.");
                        return default;
                    }
                })
                .Build();

            // Імітуємо 3 виклики поспіль при непрацюючій БД
            for (int i = 1; i <= 3; i++)
            {
                try
                {
                    breakerPipeline.Execute(() => throw new InvalidOperationException("БД не відповідає"));
                }
                catch (BrokenCircuitException)
                {
                    Console.WriteLine($"  Запит #{i}: Миттєво відхилено вимикачем (BrokenCircuitException) без навантаження на БД.");
                }
                catch (InvalidOperationException ex)
                {
                    Console.WriteLine($"  Запит #{i}: Помилка виконання ({ex.Message}).");
                }
            }

            // Чекаємо завершення BreakDuration і робимо успішний запит (Half-Open -> Closed)
            await Task.Delay(700);
            string dbData = breakerPipeline.Execute(() => "Дані успішно отримано з БД");
            Console.WriteLine($"  Запит #4: {dbData}\n");
        }

        // Сценарій 3 Polly: Комбінація Timeout + Fallback (курс валют із резервним кешем)
        static async Task DemoPollyTimeoutWithFallbackScenario()
        {
            Console.WriteLine("--- 4. Сценарій Polly #3: Комбінація Timeout + Fallback ---");

            var fallbackPipeline = new ResiliencePipelineBuilder<decimal>()
                .AddFallback(new FallbackStrategyOptions<decimal>
                {
                    ShouldHandle = new PredicateBuilder<decimal>().Handle<TimeoutRejectedException>(),
                    FallbackAction = _ => Outcome.FromResultAsValueTask(41.50m),
                    OnFallback = _ =>
                    {
                        Console.WriteLine("  [Polly Fallback] Зовнішній сервіс не встиг відповісти. Повертаємо резервний курс з кешу.");
                        return default;
                    }
                })
                .AddTimeout(new TimeoutStrategyOptions
                {
                    Timeout = TimeSpan.FromMilliseconds(250),
                    OnTimeout = _ =>
                    {
                        Console.WriteLine("  [Polly Timeout] Перевищено ліміт очікування 250 мс!");
                        return default;
                    }
                })
                .Build();

            decimal rate = await fallbackPipeline.ExecuteAsync(async ct =>
            {
                await Task.Delay(1000, ct); // Імітація «зависання» API на 1 секунду
                return 42.10m;
            });

            Console.WriteLine($"[Результат] Актуальний курс USD: {rate} грн\n");
        }
    }
}