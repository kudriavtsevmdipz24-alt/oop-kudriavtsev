using System;
using System.Globalization;

namespace IndependentWork2
{
    // Клас Product з перевантаженими конструкторами
    public class Product
    {
        // Приватні поля
        private readonly int _id;
        private readonly string _name;
        private readonly decimal _price;
        private readonly string _category;
        private readonly int _stockCount;

        // Публічні властивості (read-only)
        public int Id => _id;
        public string Name => _name;
        public decimal Price => _price;
        public string Category => _category;
        public int StockCount => _stockCount;

        // 1. Основний конструктор (ініціалізує всі поля)
        public Product(int id, string name, decimal price, string category, int stockCount)
        {
            _id = id;
            _name = name;
            _price = price;
            _category = category;
            _stockCount = stockCount;
        }

        // 2. Скорочений конструктор (викликає основний через : this(...))
        public Product(int id, string name, decimal price) 
            : this(id, name, price, "Uncategorized", 0)
        {
        }

        // 3. Конструктор копіювання (викликає основний через : this(...))
        public Product(Product other) 
            : this(other.Id, other.Name, other.Price, other.Category, other.StockCount)
        {
        }

        // Перевизначення методу ToString()
        public override string ToString()
        {
            var culture = new CultureInfo("uk-UA");
            return $"ID: {Id}, Name: {Name}, Price: {Price.ToString("C2", culture)}, Category: {Category}, Stock: {StockCount}";
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("=== Самостійна робота №2: Перевантаження конструкторів ===\n");

            // 1. Товар через основний конструктор
            Product product1 = new Product(101, "Laptop", 35000.00m, "Electronics", 15);
            Console.WriteLine($"Товар 1 (основний конструктор): {product1}");

            // 2. Товар через скорочений конструктор
            Product product2 = new Product(102, "Mouse", 800.00m);
            Console.WriteLine($"Товар 2 (скорочений конструктор): {product2}");

            // 3. Товар через конструктор копіювання
            Product product3 = new Product(product1);
            Console.WriteLine($"Товар 3 (конструктор копіювання): {product3}");
        }
    }
}
