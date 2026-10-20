using System;
using System.Collections.Generic;
using System.Linq;

namespace lab7v13
{
    // Інтерфейс сутності
    public interface IEntity
    {
        int Id { get; set; }
    }

    // Сутність за Варіантом 13: City
    public class City : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public long Population { get; set; }

        public City() { }

        public City(int id, string name, string country, long population)
        {
            Id = id;
            Name = name;
            Country = country;
            Population = population;
        }
    }

    // Узагальнений інтерфейс з where-обмеженням
    public interface IRepository<T> where T : IEntity
    {
        T GetById(int id);
        IEnumerable<T> GetAll();
        void Add(T item);
        void Update(T item);
        void Delete(int id);
    }

    // Реалізація репозиторію в пам'яті
    public class InMemoryRepository<T> : IRepository<T> where T : IEntity
    {
        private readonly List<T> _items = new List<T>();

        public void Add(T item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            if (_items.Any(i => i.Id == item.Id))
                throw new InvalidOperationException($"Елемент з ID {item.Id} вже існує.");

            _items.Add(item);
        }

        public T GetById(int id)
        {
            return _items.FirstOrDefault(i => i.Id == id);
        }

        public IEnumerable<T> GetAll()
        {
            return _items.ToList();
        }

        public void Update(T item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            var index = _items.FindIndex(i => i.Id == item.Id);
            if (index == -1)
                throw new KeyNotFoundException($"Елемент з ID {item.Id} не знайдено.");

            _items[index] = item;
        }

        public void Delete(int id)
        {
            var item = GetById(id);
            if (item == null)
                throw new KeyNotFoundException($"Елемент з ID {id} не знайдено.");

            _items.Remove(item);
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("=== Лабораторна робота №7: Generics та IRepository ===");

            IRepository<City> cityRepository = new InMemoryRepository<City>();

            // Додавання міст
            cityRepository.Add(new City(1, "Київ", "Україна", 2950000));
            cityRepository.Add(new City(2, "Львів", "Україна", 717000));
            cityRepository.Add(new City(3, "Токіо", "Японія", 13960000));

            Console.WriteLine("\n--- Список усіх міст ---");
            foreach (var city in cityRepository.GetAll())
            {
                Console.WriteLine($"ID: {city.Id} | Місто: {city.Name} ({city.Country}) | Населення: {city.Population}");
            }

            // Оновлення
            var cityToUpdate = cityRepository.GetById(2);
            if (cityToUpdate != null)
            {
                cityToUpdate.Population = 720000;
                cityRepository.Update(cityToUpdate);
                Console.WriteLine("\n[Update] Населення Львова оновлено!");
            }

            // Видалення
            cityRepository.Delete(3);
            Console.WriteLine("[Delete] Токіо видалено з репозиторію.");

            Console.WriteLine("\n--- Оновлений список міст ---");
            foreach (var city in cityRepository.GetAll())
            {
                Console.WriteLine($"ID: {city.Id} | Місто: {city.Name} ({city.Country}) | Населення: {city.Population}");
            }
        }
    }
}