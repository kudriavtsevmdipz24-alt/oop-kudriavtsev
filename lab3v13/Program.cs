using System;
using System.Threading;

namespace lab3v13
{
    // Клас за Варіантом 13: ThreadPool із реалізацією патерну Dispose
    public class CustomThreadPool : IDisposable
    {
        private bool _disposed = false;
        private bool _isActive;
        private int _poolSize;

        public CustomThreadPool(int poolSize)
        {
            _poolSize = poolSize;
            _isActive = true;
            Console.WriteLine($"[Ініціалізація] ThreadPool створено. Кількість потоків: {_poolSize}.");
        }

        public void ExecuteTask(string taskName)
        {
            if (!_isActive || _disposed)
            {
                Console.WriteLine($"[Помилка] Неможливо виконати завдання '{taskName}': ThreadPool зупинено або знищено.");
                return;
            }

            Console.WriteLine($"[Виконання] Завдання '{taskName}' виконується на одному з {_poolSize} потоків.");
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Звільнення керованих ресурсів
                    Console.WriteLine("[Dispose] Звільнення керованих ресурсів: зупинка черги завдань.");
                }

                // Звільнення некерованих ресурсів (імітація)
                if (_isActive)
                {
                    Console.WriteLine("[Dispose] Завершення всіх активних системних потоків...");
                    _isActive = false;
                }

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~CustomThreadPool()
        {
            Console.WriteLine("[Деструктор] Виклик деструктора для CustomThreadPool!");
            Dispose(false);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Сценарій 1: Використання через 'using' ===");
            using (var pool1 = new CustomThreadPool(4))
            {
                pool1.ExecuteTask("Завантаження даних");
                pool1.ExecuteTask("Обробка зображення");
            } // Dispose() викликається автматично тут

            Console.WriteLine("\n=== Сценарій 2: Явний виклик Dispose() ===");
            var pool2 = new CustomThreadPool(8);
            pool2.ExecuteTask("Фоновий розрахунок");
            pool2.Dispose(); // Явний виклик
            pool2.ExecuteTask("Нове завдання після Dispose"); // Перевірка роботи після звільнення

            Console.WriteLine("\n=== Сценарій 3: Робота деструктора через GC.Collect() ===");
            CreateAndForgetPool();

            // Примусовий виклик збирача сміття та очікування фіналізації
            Console.WriteLine("Запуск GC.Collect()...");
            GC.Collect();
            GC.WaitForPendingFinalizers();

            Console.WriteLine("\n=== Завершення роботи програми ===");
        }

        static void CreateAndForgetPool()
        {
            var pool3 = new CustomThreadPool(2);
            pool3.ExecuteTask("Коротке завдання без Dispose");
            // Об'єкт залишається без посилань після виходу з методу
        }
    }
}
