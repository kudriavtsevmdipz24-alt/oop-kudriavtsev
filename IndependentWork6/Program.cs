using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace IndependentWork6
{
    // =========================================================================
    // 1. Поліморфізм через System.Object
    // =========================================================================
    public class Product
    {
        public string Name { get; set; }
        public decimal Price { get; set; }

        public Product(string name, decimal price)
        {
            Name = name;
            Price = price;
        }

        public override string ToString()
        {
            return $"[Продукт] Назва: {Name}, Ціна: {Price} грн";
        }
    }

    public class Customer
    {
        public string FullName { get; set; }
        public string Email { get; set; }

        public Customer(string fullName, string email)
        {
            FullName = fullName;
            Email = email;
        }

        public override string ToString()
        {
            return $"[Клієнт] ПІБ: {FullName}, Email: {Email}";
        }
    }

    public class Order
    {
        public int OrderId { get; set; }
        public decimal TotalAmount { get; set; }

        public Order(int orderId, decimal totalAmount)
        {
            OrderId = orderId;
            TotalAmount = totalAmount;
        }

        public override string ToString()
        {
            return $"[Замовлення №{OrderId}] Сума: {TotalAmount} грн";
        }
    }

    // =========================================================================
    // 2. Поліморфізм через інтерфейс IEnumerable<T>
    // =========================================================================
    public class MyCustomCollection : IEnumerable<string>
    {
        private readonly List<string> _items = new List<string>();

        public void Add(string item)
        {
            _items.Add(item);
        }

        // Реалізація узагальненого GetEnumerator
        public IEnumerator<string> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        // Реалізація неузагальненого GetEnumerator
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    // =========================================================================
    // Головний клас програми
    // =========================================================================
    class Program
    {
        // 3. Поліморфна обробка абстрактного класу Stream
        public static void ProcessStream(Stream stream)
        {
            Console.WriteLine($"\nОбробка потоку типу: {stream.GetType().Name}");
            Console.WriteLine($"Чи підтримує читання: {stream.CanRead}");
            Console.WriteLine($"Довжина потоку: {stream.Length} байт(ів)");

            byte[] buffer = new byte[stream.Length];
            stream.Position = 0; // Скинути позицію на початок
            int bytesRead = stream.Read(buffer, 0, buffer.Length);

            string content = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            Console.WriteLine($"Прочитані дані: '{content}'");
        }

        static void Main(string[] args)
        {
            Console.WriteLine("=== Самостійна робота №6: Приклади поліморфізму в .NET ===\n");

            // -----------------------------------------------------------------
            // Сценарій 1: Дослідження поліморфізму через System.Object
            // -----------------------------------------------------------------
            Console.WriteLine("--- 1. Поліморфізм через System.Object (List<object>) ---");
            List<object> objects = new List<object>
            {
                new Product("Ноутбук", 35000m),
                new Customer("Олександр Шевченко", "alex@example.com"),
                new Order(1001, 35000m)
            };

            foreach (object obj in objects)
            {
                // Поліморфний виклик ToString(): версія для кожного об'єкта обирається в runtime
                Console.WriteLine(obj.ToString());
            }

            // -----------------------------------------------------------------
            // Сценарій 2: Дослідження поліморфізму через IEnumerable<T>
            // -----------------------------------------------------------------
            Console.WriteLine("\n--- 2. Поліморфізм через IEnumerable<string> (MyCustomCollection) ---");
            MyCustomCollection myCollection = new MyCustomCollection();
            myCollection.Add("Елемент #1: Поліморфізм");
            myCollection.Add("Елемент #2: Інтерфейси");
            myCollection.Add("Елемент #3: Анонімні обхідники");

            // Обхід колекції завдяки уніфікованому інтерфейсу IEnumerable
            foreach (string item in myCollection)
            {
                Console.WriteLine($"-> {item}");
            }

            // -----------------------------------------------------------------
            // Сценарій 3: Дослідження поліморфізму через абстрактний клас Stream
            // -----------------------------------------------------------------
            Console.WriteLine("\n--- 3. Поліморфізм через абстрактний клас Stream ---");

            // а) Робота з MemoryStream
            byte[] sampleData = Encoding.UTF8.GetBytes("Дані з пам'яті (MemoryStream)");
            using (MemoryStream memoryStream = new MemoryStream(sampleData))
            {
                ProcessStream(memoryStream);
            }

            // б) Робота з FileStream
            string filePath = "test_data.txt";
            File.WriteAllText(filePath, "Дані з файлу (FileStream)");

            using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                ProcessStream(fileStream);
            }

            // Видалення тимчасового тестового файлу
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}
