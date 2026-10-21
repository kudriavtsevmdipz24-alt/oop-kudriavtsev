using System;
using System.Collections.Generic;

namespace lab8v13
{
    // Кастомний клас сутності для демонстрації
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }

        public Product(int id, string name, string category)
        {
            Id = id;
            Name = name;
            Category = category;
        }
    }

    // Узагальнений клас-утиліта
    public static class CollectionHelper
    {
        // Метод DistinctBy за Варіантом 13 (власна реалізація через HashSet без LINQ)
        public static IEnumerable<T> DistinctBy<T, TKey>(IEnumerable<T> collection, Func<T, TKey> keySelector)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));
            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            HashSet<TKey> knownKeys = new HashSet<TKey>();

            foreach (T item in collection)
            {
                TKey key = keySelector(item);
                // HashSet.Add повертає true, якщо ключ додано вперше
                if (knownKeys.Add(key))
                {
                    yield return item;
                }
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Лабораторна робота №8: Делегати та CollectionHelper<T> ===");

            // 1. Демонстрація на колекції рядків List<string> (унікальність за першою літерою)
            Console.WriteLine("\n--- 1. Рядки: Отримання унікальних елементів за першою літерою ---");
            List<string> words = new List<string> { "Apple", "Banana", "Avocado", "Cherry", "Blueberry", "Cranberry" };

            var distinctByFirstLetter = CollectionHelper.DistinctBy(words, word => char.ToUpper(word[0]));

            Console.WriteLine("Початковий список: " + string.Join(", ", words));
            Console.WriteLine("Унікальні перші літери:");
            foreach (var item in distinctByFirstLetter)
            {
                Console.WriteLine($" -> {item} (Перша літера: {char.ToUpper(item[0])})");
            }

            // 2. Демонстрація на колекції об'єктів List<Product> (унікальність за категорією)
            Console.WriteLine("\n--- 2. Продукти: Отримання унікальних продуктів за категорією ---");
            List<Product> products = new List<Product>
            {
                new Product(1, "Ноутбук", "Електроніка"),
                new Product(2, "Смартфон", "Електроніка"),
                new Product(3, "Яблуко", "Продукти"),
                new Product(4, "Монітор", "Електроніка"),
                new Product(5, "Хліб", "Продукти"),
                new Product(6, "Футболка", "Одяг")
            };

            var distinctByCategory = CollectionHelper.DistinctBy(products, p => p.Category);

            Console.WriteLine("Представники кожної унікальної категорії:");
            foreach (var product in distinctByCategory)
            {
                Console.WriteLine($" -> [{product.Category}] Продукт: {product.Name} (ID: {product.Id})");
            }
        }
    }
}
