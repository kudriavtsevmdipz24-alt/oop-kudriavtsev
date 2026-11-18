using System;
using System.Collections.Generic;

namespace IndependentWork15
{
    public static class CollectionUtils
    {
        // 1. Map (аналог LINQ Select)
        public static IEnumerable<TResult> Map<TSource, TResult>(
            IEnumerable<TSource> collection,
            Func<TSource, TResult> mapper)
        {
            foreach (var item in collection)
                yield return mapper(item);
        }

        // 2. MaxBy
        public static T MaxBy<T, TKey>(
            IEnumerable<T> collection,
            Func<T, TKey> keySelector)
            where TKey : IComparable<TKey>
        {
            T maxItem = default!;
            TKey maxKey = default!;
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

            if (first) throw new InvalidOperationException("Колекція порожня.");
            return maxItem;
        }

        // 3. Filter (аналог LINQ Where)
        public static IEnumerable<T> Filter<T>(
            IEnumerable<T> collection,
            Predicate<T> predicate)
        {
            foreach (var item in collection)
                if (predicate(item))
                    yield return item;
        }

        // 4. ForEach
        public static void ForEach<T>(
            IEnumerable<T> collection,
            Action<T> action)
        {
            foreach (var item in collection)
                action(item);
        }
    }

    public record Product(string Name, decimal Price);

    class Program
    {
        static void Main()
        {
            Console.WriteLine("=== Самостійна робота №15: CollectionUtils (Делегати та лямбда-вирази) ===\n");

            // 1. Колекція об'єктів List<Product>
            Console.WriteLine("--- 1. List<Product> ---");
            var products = new List<Product>
            {
                new("Laptop", 32000m),
                new("Mouse", 750m),
                new("Keyboard", 2400m),
                new("Monitor", 11500m)
            };

            var names = CollectionUtils.Map(products, p => p.Name);
            Console.WriteLine("Назви (Map): " + string.Join(", ", names));

            var topProduct = CollectionUtils.MaxBy(products, p => p.Price);
            Console.WriteLine($"Найдорожчий (MaxBy): {topProduct.Name} — {topProduct.Price} грн");

            var expensive = CollectionUtils.Filter(products, p => p.Price > 2000m);
            Console.Write("Дорожчі за 2000 грн (Filter + ForEach): ");
            CollectionUtils.ForEach(expensive, p => Console.Write($"{p.Name} ({p.Price} грн); "));
            Console.WriteLine("\n");

            // 2. Колекція чисел List<int>
            Console.WriteLine("--- 2. List<int> ---");
            var numbers = new List<int> { 4, 15, 8, 23, 42, 11 };

            var doubled = CollectionUtils.Map(numbers, n => n * 2);
            Console.WriteLine("Подвоєні (Map): " + string.Join(", ", doubled));

            var evens = CollectionUtils.Filter(numbers, n => n % 2 == 0);
            Console.WriteLine("Парні (Filter): " + string.Join(", ", evens));
            Console.WriteLine($"Максимум (MaxBy): {CollectionUtils.MaxBy(numbers, n => n)}\n");

            // 3. Колекція рядків List<string>
            Console.WriteLine("--- 3. List<string> ---");
            var langs = new List<string> { "C#", "Python", "TypeScript", "Go", "Rust" };

            var upper = CollectionUtils.Map(langs, s => s.ToUpper());
            Console.WriteLine("Верхній регістр (Map): " + string.Join(", ", upper));

            var longest = CollectionUtils.MaxBy(langs, s => s.Length);
            Console.WriteLine($"Найдовша назва (MaxBy): {longest} ({longest.Length} симв.)");

            var shortNames = CollectionUtils.Filter(langs, s => s.Length <= 4);
            Console.Write("Короткі назви <= 4 симв. (ForEach): ");
            CollectionUtils.ForEach(shortNames, s => Console.Write($"[{s}] "));
            Console.WriteLine();
        }
    }
}
