using System;
using System.Collections.Generic;

namespace IndependentWork10
{
    // 1. Узагальнений клас утиліт CollectionUtils
    public static class CollectionUtils
    {
        // Метод 1: Map (Аналог LINQ Select)
        public static IEnumerable<TResult> Map<TSource, TResult>(
            IEnumerable<TSource> collection,
            Func<TSource, TResult> mapper)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (mapper == null) throw new ArgumentNullException(nameof(mapper));

            var result = new List<TResult>();
            foreach (var item in collection)
            {
                result.Add(mapper(item));
            }
            return result;
        }

        // Метод 2: MaxBy
        public static T MaxBy<T, TKey>(
            IEnumerable<T> collection,
            Func<T, TKey> keySelector)
            where TKey : IComparable<TKey>
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));

            T maxItem = default;
            TKey maxKey = default;
            bool first = true;

            foreach (var item in collection)
            {
                TKey key = keySelector(item);
                if (first || key.CompareTo(maxKey) > 0)
                {
                    maxKey = key;
                    maxItem = item;
                    first = false;
                }
            }

            if (first)
            {
                throw new InvalidOperationException("Колекція порожня.");
            }

            return maxItem;
        }

        // Метод 3: Filter (Аналог LINQ Where)
        public static IEnumerable<T> Filter<T>(
            IEnumerable<T> collection,
            Predicate<T> predicate)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            var result = new List<T>();
            foreach (var item in collection)
            {
                if (predicate(item))
                {
                    result.Add(item);
                }
            }
            return result;
        }

        // Метод 4: ForEach (Аналог List<T>.ForEach)
        public static void ForEach<T>(
            IEnumerable<T> collection,
            Action<T> action)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (action == null) throw new ArgumentNullException(nameof(action));

            foreach (var item in collection)
            {
                action(item);
            }
        }
    }

    // Клас сутності для тестування
    public class Product
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; }

        public override string ToString() => $"{Name} ({Category}) — {Price} грн";
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Самостійна робота №10: Власні утиліти для колекцій ===\n");

            // -----------------------------------------------------------------
            // Тест 1: Робота з List<Product>
            // -----------------------------------------------------------------
            Console.WriteLine("--- 1. Тестування на колекції продуктів List<Product> ---");
            var products = new List<Product>
            {
                new Product { Name = "Ноутбук ASUS", Price = 32000m, Category = "Електроніка" },
                new Product { Name = "Мишка Logitech", Price = 850m, Category = "Аксесуари" },
                new Product { Name = "Клавіатура Keychron", Price = 3800m, Category = "Аксесуари" },
                new Product { Name = "Монітор Dell", Price = 12500m, Category = "Електроніка" }
            };

            // Map: Отримання назв продуктів
            var productNames = CollectionUtils.Map(products, p => p.Name);
            Console.WriteLine("Назви продуктів: " + string.Join(", ", productNames));

            // MaxBy: Найдорожчий продукт
            var mostExpensive = CollectionUtils.MaxBy(products, p => p.Price);
            Console.WriteLine($"Найдорожчий продукт: {mostExpensive.Name} ({mostExpensive.Price} грн)");

            // Filter: Продукти дорожчі за 3000 грн
            var expensiveProducts = CollectionUtils.Filter(products, p => p.Price > 3000m);
            Console.WriteLine("Продукти дорожчі за 3000 грн:");
            CollectionUtils.ForEach(expensiveProducts, p => Console.WriteLine($" -> {p}"));

            // -----------------------------------------------------------------
            // Тест 2: Робота з List<int>
            // -----------------------------------------------------------------
            Console.WriteLine("\n--- 2. Тестування на колекції чисел List<int> ---");
            var numbers = new List<int> { 12, 45, 7, 89, 23, 56, 3, 90 };

            // Map: Квадрати чисел
            var squares = CollectionUtils.Map(numbers, n => n * n);
            Console.WriteLine("Квадрати чисел: " + string.Join(", ", squares));

            // Filter: Парні числа
            var evenNumbers = CollectionUtils.Filter(numbers, n => n % 2 == 0);
            Console.Write("Парні числа: ");
            CollectionUtils.ForEach(evenNumbers, n => Console.Write($"{n} "));
            Console.WriteLine();

            // MaxBy: Число з найбільшим остачником від ділення на 10
            var maxRemainder = CollectionUtils.MaxBy(numbers, n => n % 10);
            Console.WriteLine($"Число з найбільшою останньою цифрою: {maxRemainder}");

            // -----------------------------------------------------------------
            // Тест 3: Робота з List<string>
            // -----------------------------------------------------------------
            Console.WriteLine("\n--- 3. Тестування на колекції рядків List<string> ---");
            var words = new List<string> { "C#", "Делегати", "Лямбда", "Поліморфізм", "ООП", "Колекції" };

            // Map: Довжини слів
            var wordLengths = CollectionUtils.Map(words, w => $"{w} ({w.Length} слів)");
            Console.WriteLine("Довжини слів: " + string.Join(", ", wordLengths));

            // MaxBy: Найдовше слово
            var longestWord = CollectionUtils.MaxBy(words, w => w.Length);
            Console.WriteLine($"Найдовше слово: '{longestWord}'");

            // Filter: Слова довжиною більше 5 символів
            var longWords = CollectionUtils.Filter(words, w => w.Length > 5);
            Console.WriteLine("Слова довше 5 символів:");
            CollectionUtils.ForEach(longWords, w => Console.WriteLine($" -> {w}"));
        }
    }
}