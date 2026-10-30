using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IndependentWork3
{
    // Клас Histogram за Варіантом 13
    public class Histogram
    {
        // Приватне поле для зберігання категорій та їх кількості
        private readonly Dictionary<string, int> _data = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        // 1. Індексатор для доступу до елементів за назвою категорії
        public int this[string category]
        {
            get
            {
                if (string.IsNullOrWhiteSpace(category))
                    return 0;

                return _data.TryGetValue(category, out int count) ? count : 0;
            }
            set
            {
                if (string.IsNullOrWhiteSpace(category))
                    return;

                _data[category] = Math.Max(0, value);
            }
        }

        // Додатковий метод: Збільшення значення категорії на 1
        public void Increment(string category)
        {
            if (string.IsNullOrWhiteSpace(category))
                return;

            this[category]++;
        }

        // 2. Перевантаження оператора + (об'єднання двох гістограм)
        public static Histogram operator +(Histogram h1, Histogram h2)
        {
            var result = new Histogram();

            if (h1 != null)
            {
                foreach (var pair in h1._data)
                    result[pair.Key] += pair.Value;
            }

            if (h2 != null)
            {
                foreach (var pair in h2._data)
                    result[pair.Key] += pair.Value;
            }

            return result;
        }

        // 3. Перевантаження операторів == та !=
        public static bool operator ==(Histogram h1, Histogram h2)
        {
            if (ReferenceEquals(h1, h2))
                return true;

            if (h1 is null || h2 is null)
                return false;

            return h1.Equals(h2);
        }

        public static bool operator !=(Histogram h1, Histogram h2)
        {
            return !(h1 == h2);
        }

        // 4. Перевизначення Equals()
        public override bool Equals(object obj)
        {
            if (obj is not Histogram other)
                return false;

            if (_data.Count != other._data.Count)
                return false;

            foreach (var pair in _data)
            {
                if (!other._data.TryGetValue(pair.Key, out int otherValue) || pair.Value != otherValue)
                    return false;
            }

            return true;
        }

        // 5. Перевизначення GetHashCode()
        public override int GetHashCode()
        {
            int hash = 17;
            foreach (var pair in _data.OrderBy(p => p.Key))
            {
                hash = hash * 31 + pair.Key.ToLower().GetHashCode();
                hash = hash * 31 + pair.Value.GetHashCode();
            }
            return hash;
        }

        // 6. Перевизначення ToString()
        public override string ToString()
        {
            if (_data.Count == 0)
                return "[Порожня гістограма]";

            var sb = new StringBuilder();
            foreach (var pair in _data)
            {
                sb.AppendLine($" - Категорія '{pair.Key}': {pair.Value}");
            }
            return sb.ToString().TrimEnd();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Самостійна робота №3: Індексатори та перевантаження операторів ===");
            Console.WriteLine("=== Варіант 13: Клас Histogram ===\n");

            // Створення об'єкта h1 та використання індексатора
            Console.WriteLine("--- 1. Демонстрація індексатора та методу Increment ---");
            Histogram h1 = new Histogram();
            h1["Фрукти"] = 5;
            h1["Овочі"] = 12;
            h1.Increment("Фрукти"); // збільшуємо на 1

            Console.WriteLine("Гістограма 1 (h1):");
            Console.WriteLine(h1);
            Console.WriteLine($"Запит через індексатор h1[\"Фрукти\"]: {h1["Фрукти"]}");

            // Створення об'єкта h2
            Console.WriteLine("\n--- 2. Створення Гістограми 2 (h2) ---");
            Histogram h2 = new Histogram();
            h2["Овочі"] = 3;
            h2["Напої"] = 8;
            Console.WriteLine("Гістограма 2 (h2):");
            Console.WriteLine(h2);

            // Демонстрація оператора +
            Console.WriteLine("\n--- 3. Демонстрація оператора + (h3 = h1 + h2) ---");
            Histogram h3 = h1 + h2;
            Console.WriteLine("Результат об'єднання (h3):");
            Console.WriteLine(h3);

            // Демонстрація операторів == та !=
            Console.WriteLine("\n--- 4. Демонстрація операторів == та != ---");
            Histogram h4 = new Histogram();
            h4["Фрукти"] = 6;
            h4["Овочі"] = 12;

            Console.WriteLine($"Чи h1 == h2? {h1 == h2}");
            Console.WriteLine($"Чи h1 == h4? {h1 == h4}");
            Console.WriteLine($"Чи h1 != h2? {h1 != h2}");
        }
    }
}
