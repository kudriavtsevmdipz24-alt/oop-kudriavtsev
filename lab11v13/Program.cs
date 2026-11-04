using System;
using System.Collections.Generic;
using System.Linq;

namespace lab11v13
{
    // 1. Сутності даних (Варіант 13)
    public class CarOption
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }

        public CarOption(int id, string name, decimal price)
        {
            Id = id;
            Name = name;
            Price = price;
        }
    }

    public class Car
    {
        public int Id { get; set; }
        public string Model { get; set; }
        public decimal BasePrice { get; set; }
        public List<CarOption> Options { get; set; } = new List<CarOption>();

        public Car(int id, string model, decimal basePrice, List<CarOption> options = null)
        {
            Id = id;
            Model = model;
            BasePrice = basePrice;
            if (options != null)
            {
                Options = options;
            }
        }
    }

    public class Dealership
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Car> Cars { get; set; } = new List<Car>();

        public Dealership(int id, string name, List<Car> cars = null)
        {
            Id = id;
            Name = name;
            if (cars != null)
            {
                Cars = cars;
            }
        }
    }

    // 2. Метод розширення (Custom Extension Method за Варіантом 13)
    public static class CarExtensions
    {
        // Обчислює загальну ціну автомобіля з урахуванням базової ціни та всіх додаткових опцій
        public static decimal TotalCarPrice(this Car car)
        {
            if (car == null)
                return 0;

            decimal optionsTotal = car.Options != null ? car.Options.Sum(o => o.Price) : 0;
            return car.BasePrice + optionsTotal;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Лабораторна робота №11: LINQ: SelectMany, пагінація й custom extensions ===");
            Console.WriteLine("=== Варіант 13: Автосалони та їхні автомобілі з опціями ===\n");

            // Створення додаткових опцій
            var optLeather = new CarOption(1, "Шкіряний салон", 1500m);
            var optSunroof = new CarOption(2, "Панорамний дах", 1200m);
            var optAudio = new CarOption(3, "Преміум аудіосистема JBL", 800m);
            var optAutopilot = new CarOption(4, "Автопілот / Автопарковка", 3000m);
            var optHeatedSeats = new CarOption(5, "Підігрів усіх сидінь", 500m);

            // Створення автомобілів з опціями (більше 10 об'єктів для випробування пагінації)
            var carsGroup1 = new List<Car>
            {
                new Car(101, "Toyota Camry", 30000m, new List<CarOption> { optLeather, optHeatedSeats }),
                new Car(102, "Toyota RAV4", 34000m, new List<CarOption> { optSunroof, optAudio }),
                new Car(103, "Toyota Corolla", 22000m, new List<CarOption> { optHeatedSeats }),
                new Car(104, "Lexus RX 350", 55000m, new List<CarOption> { optLeather, optSunroof, optAudio, optAutopilot }),
                new Car(105, "Lexus ES 300h", 48000m, new List<CarOption> { optLeather, optAudio })
            };

            var carsGroup2 = new List<Car>
            {
                new Car(201, "BMW X5", 65000m, new List<CarOption> { optLeather, optSunroof, optAutopilot }),
                new Car(202, "BMW M3", 72000m, new List<CarOption> { optAudio, optLeather }),
                new Car(203, "BMW 3 Series", 41000m, new List<CarOption> { optHeatedSeats }),
                new Car(204, "BMW X3", 49000m, new List<CarOption> { optSunroof, optHeatedSeats }),
                new Car(205, "BMW i4", 56000m, new List<CarOption> { optAutopilot, optAudio })
            };

            var carsGroup3 = new List<Car>
            {
                new Car(301, "Audi Q7", 68000m, new List<CarOption> { optLeather, optSunroof, optAudio }),
                new Car(302, "Audi A6", 52000m, new List<CarOption> { optHeatedSeats, optAudio })
            };

            // Створення автосалонів
            List<Dealership> dealerships = new List<Dealership>
            {
                new Dealership(1, "Автосалон 'Тойота Центр'", carsGroup1),
                new Dealership(2, "Автосалон 'Баварія Моторс'", carsGroup2),
                new Dealership(3, "Автосалон 'Ауді Альянс'", carsGroup3)
            };

            // -----------------------------------------------------------------
            // 1. SelectMany: Отримати всі опції всіх автомобілів усіх автосалонів
            // -----------------------------------------------------------------
            Console.WriteLine("--- 1. SelectMany: Розгортання двох рівнів вкладених колекцій ---");

            // Підхід з двома SelectMany для вилучення CarOption з Dealerships -> Cars -> Options
            var allOptions = dealerships
                .SelectMany(d => d.Cars)
                .SelectMany(c => c.Options)
                .ToList();

            Console.WriteLine($"Всього опцій у базі даних (із дублікатами): {allOptions.Count}");
            Console.WriteLine("Перелік перших 6 вилучених опцій:");
            foreach (var opt in allOptions.Take(6))
            {
                Console.WriteLine($" -> [{opt.Id}] {opt.Name} (Вартість: {opt.Price:C0})");
            }

            // -----------------------------------------------------------------
            // 2. Custom Extension Method: Демонстрація TotalCarPrice()
            // -----------------------------------------------------------------
            Console.WriteLine("\n--------------------------------------------------");
            Console.WriteLine("--- 2. Custom Extension Method: Перевірка TotalCarPrice() ---");
            Car testCar = carsGroup1[0]; // Toyota Camry
            Console.WriteLine($"Автомобіль: {testCar.Model}");
            Console.WriteLine($"Базова ціна: {testCar.BasePrice:C0}");
            Console.WriteLine($"Опції: {string.Join(", ", testCar.Options.Select(o => o.Name))}");
            Console.WriteLine($"Загальна вартість (з опціями через розширення): {testCar.TotalCarPrice():C0}");

            // -----------------------------------------------------------------
            // 3. Пагінація (Skip, Take): Посторінковий вивід усіх автомобілів
            // -----------------------------------------------------------------
            Console.WriteLine("\n--------------------------------------------------");
            Console.WriteLine("--- 3. Пагінація (Skip / Take): Вивід автомобілів по 5 на сторінку ---");

            // Отримуємо плаский список усіх автомобілів з усіх автосалонів
            var allCars = dealerships.SelectMany(d => d.Cars).ToList();

            int pageSize = 5;
            int totalCars = allCars.Count;
            int totalPages = (int)Math.Ceiling((double)totalCars / pageSize);

            for (int pageIndex = 0; pageIndex < totalPages; pageIndex++)
            {
                Console.WriteLine($"\n=== СТОРІНКА {pageIndex + 1} з {totalPages} ===");
                var currentPageCars = allCars
                    .Skip(pageIndex * pageSize)
                    .Take(pageSize)
                    .ToList();

                foreach (var car in currentPageCars)
                {
                    Console.WriteLine($" -> Model: {car.Model,-16} | Base: {car.BasePrice,7:C0} | Total: {car.TotalCarPrice(),7:C0} | Options: {car.Options.Count}");
                }
            }
        }
    }
}
