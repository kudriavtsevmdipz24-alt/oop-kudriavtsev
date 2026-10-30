using System;

namespace IndependentWork4
{
    // Демонстрація концепцій інкапсуляції та валідації на основі аналізу Entity Framework Core
    public class EntityEntryDemo
    {
        private object _entity;
        private string _state;

        public object Entity => _entity; // Read-only властивість

        public string State
        {
            get => _state;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Стан сутності не може бути порожнім.", nameof(value));
                _state = value;
            }
        }

        public EntityEntryDemo(object entity, string initialState)
        {
            _entity = entity ?? throw new ArgumentNullException(nameof(entity));
            State = initialState;
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("=== Самостійна робота №4: Аналіз інкапсуляції в Open-Source проєктах ===\n");

            try
            {
                var demo = new EntityEntryDemo(new { Id = 1, Name = "Тестовий об'єкт" }, "Added");
                Console.WriteLine($"Сутність: {demo.Entity}");
                Console.WriteLine($"Поточний стан: {demo.State}");

                Console.WriteLine("\n[Тест валідації] Спроба встановити порожній стан...");
                demo.State = "";
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"[Успішно перехоплено виняток валідації]: {ex.Message}");
            }

            Console.WriteLine("\nДетальний звіт із аналізом Entity Framework Core та AutoMapper сформовано в README.md");
        }
    }
}
