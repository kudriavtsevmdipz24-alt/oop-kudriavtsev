using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace IndependentWork12
{
    public record Product(int Id, string Name, string Category, decimal UnitPrice);
    public record Customer(int Id, string Name, string City);
    public record Sale(int Id, int ProductId, int Quantity, DateTime SaleDate, int CustomerId);

    class Program
    {
        static List<Product> _products = new();
        static List<Customer> _customers = new();
        static List<Sale> _sales = new();

        static void Main()
        {
            Console.WriteLine("=== Самостійна робота №12: LINQ pipelines: ETL та аналітика ===\n");
            GenerateData(100);

            TopProductsByCategory();
            TotalRevenueByCity();
            SalesByYearAndMonth();
            TopVipCustomers();
        }

        // Генерація тестових даних (100 елементів кожної сутності)
        static void GenerateData(int count)
        {
            var rnd = new Random(42);
            var categories = new[] { "Electronics", "Clothing", "Books", "Home", "Sports" };
            var cities = new[] { "Kyiv", "Lviv", "Rivne", "Odesa", "Kharkiv" };

            _products = Enumerable.Range(1, count)
                .Select(i => new Product(i, $"Product_{i}", categories[rnd.Next(categories.Length)], rnd.Next(100, 5000)))
                .ToList();

            _customers = Enumerable.Range(1, count)
                .Select(i => new Customer(i, $"Customer_{i}", cities[rnd.Next(cities.Length)]))
                .ToList();

            _sales = Enumerable.Range(1, count * 2) // 200 продажів для репрезентативності
                .Select(i => new Sale(
                    i,
                    rnd.Next(1, count + 1),
                    rnd.Next(1, 10),
                    new DateTime(rnd.Next(2024, 2027), rnd.Next(1, 13), rnd.Next(1, 28)),
                    rnd.Next(1, count + 1)))
                .ToList();
        }

        // Пайплайн 1: Топ-3 найпопулярніші продукти за категоріями
        static void TopProductsByCategory()
        {
            var sw = Stopwatch.StartNew();

            var result = _sales
                .GroupBy(s => s.ProductId)
                .Select(g => new { ProductId = g.Key, TotalQty = g.Sum(s => s.Quantity) })
                .Join(_products, g => g.ProductId, p => p.Id, (g, p) => new { p.Category, p.Name, g.TotalQty })
                .GroupBy(x => x.Category)
                .Select(g => new
                {
                    Category = g.Key,
                    Top3 = g.OrderByDescending(x => x.TotalQty).Take(3).ToList()
                })
                .OrderBy(x => x.Category)
                .ToList();

            sw.Stop();
            Console.WriteLine($"--- 1. Топ-3 продукти за категоріями (Час: {sw.ElapsedTicks} ticks) ---");
            foreach (var cat in result)
            {
                var topStr = string.Join(", ", cat.Top3.Select(p => $"{p.Name} ({p.TotalQty} шт.)"));
                Console.WriteLine($"  {cat.Category,-12}: {topStr}");
            }
            Console.WriteLine();
        }

        // Пайплайн 2: Загальний дохід за містами клієнтів
        static void TotalRevenueByCity()
        {
            var sw = Stopwatch.StartNew();

            var result = _sales
                .Join(_products, s => s.ProductId, p => p.Id, (s, p) => new { s.CustomerId, Revenue = s.Quantity * p.UnitPrice })
                .Join(_customers, sp => sp.CustomerId, c => c.Id, (sp, c) => new { c.City, sp.Revenue })
                .GroupBy(x => x.City)
                .Select(g => new { City = g.Key, TotalRevenue = g.Sum(x => x.Revenue) })
                .OrderByDescending(x => x.TotalRevenue)
                .ToList();

            sw.Stop();
            Console.WriteLine($"--- 2. Загальний дохід за містами (Час: {sw.ElapsedTicks} ticks) ---");
            foreach (var item in result)
                Console.WriteLine($"  {item.City,-10}: {item.TotalRevenue:N2} грн");
            Console.WriteLine();
        }

        // Пайплайн 3: Продажі за роками та місяцями
        static void SalesByYearAndMonth()
        {
            var sw = Stopwatch.StartNew();

            var result = _sales
                .Join(_products, s => s.ProductId, p => p.Id, (s, p) => new { s.SaleDate.Year, s.SaleDate.Month, s.Quantity, Revenue = s.Quantity * p.UnitPrice })
                .GroupBy(x => new { x.Year, x.Month })
                .Select(g => new
                {
                    g.Key.Year,
                    g.Key.Month,
                    TotalUnits = g.Sum(x => x.Quantity),
                    TotalRevenue = g.Sum(x => x.Revenue)
                })
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .ToList();

            sw.Stop();
            Console.WriteLine($"--- 3. Динаміка продажів за роками/місяцями (Час: {sw.ElapsedTicks} ticks) ---");
            foreach (var item in result.Take(6)) // перші 6 періодів для компактності виводу
                Console.WriteLine($"  {item.Year}-{item.Month:D2}: {item.TotalUnits,3} од. | Дохід: {item.TotalRevenue,10:N2} грн");
            Console.WriteLine($"  ... (всього періодів: {result.Count})\n");
        }

        // Пайплайн 4 (власний): Топ-5 VIP-клієнтів із найбільшим середнім чеком за 2025-2026 роки
        static void TopVipCustomers()
        {
            var sw = Stopwatch.StartNew();

            var result = _sales
                .Where(s => s.SaleDate.Year >= 2025)
                .Join(_products, s => s.ProductId, p => p.Id, (s, p) => new { s.CustomerId, Amount = s.Quantity * p.UnitPrice })
                .GroupBy(x => x.CustomerId)
                .Select(g => new { CustomerId = g.Key, OrdersCount = g.Count(), TotalSpent = g.Sum(x => x.Amount) })
                .Join(_customers, g => g.CustomerId, c => c.Id, (g, c) => new { c.Name, c.City, g.OrdersCount, g.TotalSpent })
                .OrderByDescending(x => x.TotalSpent)
                .Take(5)
                .ToList();

            sw.Stop();
            Console.WriteLine($"--- 4. Топ-5 VIP-клієнтів з 2025 року (Час: {sw.ElapsedTicks} ticks) ---");
            foreach (var vip in result)
                Console.WriteLine($"  {vip.Name,-12} ({vip.City,-7}): Замовлень: {vip.OrdersCount} | Сума: {vip.TotalSpent:N2} грн");
            Console.WriteLine();
        }
    }
}
