using System;
using System.Collections.Generic;

namespace IndependentWork8
{
    // Узагальнений клас кешу з політикою витіснення FIFO
    public class Cache<TKey, TValue>
    {
        private readonly int _capacity;
        private readonly Dictionary<TKey, TValue> _cache;
        private readonly Queue<TKey> _keysOrder;

        public int Count => _cache.Count;

        public Cache(int capacity)
        {
            if (capacity <= 0)
                throw new ArgumentException("Ємність кешу повинна бути більшою за 0.", nameof(capacity));

            _capacity = capacity;
            _cache = new Dictionary<TKey, TValue>();
            _keysOrder = new Queue<TKey>();
        }

        public void Add(TKey key, TValue value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            // Якщо ключ уже існує, оновлюємо значення без зміни черги
            if (_cache.ContainsKey(key))
            {
                _cache[key] = value;
                return;
            }

            // Якщо кеш повний, видаляємо найстаріший елемент за політикою FIFO
            if (_cache.Count >= _capacity)
            {
                TKey oldestKey = _keysOrder.Dequeue();
                _cache.Remove(oldestKey);
            }

            _cache[key] = value;
            _keysOrder.Enqueue(key);
        }

        public TValue Get(TKey key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (_cache.TryGetValue(key, out TValue value))
            {
                return value;
            }

            throw new KeyNotFoundException($"Ключ '{key}' не знайдено в кеші.");
        }

        public bool ContainsKey(TKey key)
        {
            if (key == null)
                return false;

            return _cache.ContainsKey(key);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Самостійна робота №8: Узагальнений кеш Cache<TKey, TValue> із тестами ===\n");

            // Демонстрація роботи кешу (ємність = 3)
            Cache<string, string> cache = new Cache<string, string>(3);

            Console.WriteLine("--- 1. Додавання 3 елементів у кеш (ємність = 3) ---");
            cache.Add("item1", "Ноутбук");
            cache.Add("item2", "Мишка");
            cache.Add("item3", "Клавіатура");

            Console.WriteLine($"Поточний Count: {cache.Count}");
            Console.WriteLine($"Отримання 'item1': {cache.Get("item1")}");

            Console.WriteLine("\n--- 2. Переповнення кешу (додаємо 4-й елемент 'item4') ---");
            cache.Add("item4", "Монітор");

            Console.WriteLine($"Чи існує 'item1' (найстаріший)? {cache.ContainsKey("item1")} (видалено за FIFO)");
            Console.WriteLine($"Чи існує 'item4' (новий)? {cache.ContainsKey("item4")}");
            Console.WriteLine($"Поточний Count: {cache.Count}");

            try
            {
                Console.WriteLine("\nСпроба отримати видалений 'item1':");
                cache.Get("item1");
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"[Успішно перехоплено виняток]: {ex.Message}");
            }
        }
    }
}
