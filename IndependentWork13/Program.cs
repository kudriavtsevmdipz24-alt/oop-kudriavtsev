using System;
using System.Collections.Generic;

namespace IndependentWork13
{
    public class Cache<TKey, TValue> where TKey : notnull
    {
        private readonly int _capacity;
        private readonly Dictionary<TKey, TValue> _cache = new();
        private readonly Queue<TKey> _keysOrder = new();

        public int Count => _cache.Count;

        public Cache(int capacity)
        {
            if (capacity <= 0)
                throw new ArgumentException("Ємність має бути більшою за 0.", nameof(capacity));
            _capacity = capacity;
        }

        public void Add(TKey key, TValue value)
        {
            if (_cache.ContainsKey(key))
            {
                _cache[key] = value;
                return;
            }

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
            if (_cache.TryGetValue(key, out TValue? value))
                return value;

            throw new KeyNotFoundException($"Ключ '{key}' не знайдено в кеші.");
        }

        public bool ContainsKey(TKey key) => _cache.ContainsKey(key);
    }

    class Program
    {
        static void Main()
        {
            Console.WriteLine("=== Самостійна робота №13: Узагальнений FIFO-кеш Cache<TKey, TValue> ===\n");

            var cache = new Cache<string, string>(3);
            cache.Add("k1", "Дані 1");
            cache.Add("k2", "Дані 2");
            cache.Add("k3", "Дані 3");

            Console.WriteLine($"Count після 3 додавань: {cache.Count}");
            Console.WriteLine($"Get('k1'): {cache.Get("k1")}");

            Console.WriteLine("\nДодаємо 'k4' (перевищення ємності 3)...");
            cache.Add("k4", "Дані 4");

            Console.WriteLine($"ContainsKey('k1') (витіснено за FIFO): {cache.ContainsKey("k1")}");
            Console.WriteLine($"ContainsKey('k4') (новий елемент): {cache.ContainsKey("k4")}");

            try
            {
                cache.Get("k1");
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"Перехоплено виняток: {ex.Message}");
            }
        }
    }
}
