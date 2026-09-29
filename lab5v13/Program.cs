using System;
using System.Collections.Generic;
using System.Linq;

namespace lab5v13
{
    // Базовый класс
    public class Sensor
    {
        public string Location { get; set; }

        public Sensor(string location)
        {
            Location = location;
        }

        // Виртуальный метод, возвращающий значение показателя
        public virtual double GetReading()
        {
            return 0.0;
        }
    }

    // Породистый класс 1: Датчик температуры
    public class TemperatureSensor : Sensor
    {
        public string Unit { get; set; } // Celsius / Fahrenheit
        private double _temperature;

        public TemperatureSensor(string location, string unit, double temperature) 
            : base(location)
        {
            Unit = unit;
            _temperature = temperature;
        }

        public override double GetReading()
        {
            Console.WriteLine($"[TemperatureSensor] Локация: {Location} | Температура: {_temperature} °{Unit}");
            return _temperature;
        }
    }

    // Породистый класс 2: Датчик давления
    public class PressureSensor : Sensor
    {
        public double MaxPressure { get; set; }
        private double _currentPressure;

        public PressureSensor(string location, double maxPressure, double currentPressure) 
            : base(location)
        {
            MaxPressure = maxPressure;
            _currentPressure = currentPressure;
        }

        public override double GetReading()
        {
            Console.WriteLine($"[PressureSensor] Локация: {Location} | Давление: {_currentPressure} кПа (Макс: {MaxPressure} кПа)");
            return _currentPressure;
        }
    }

    // Породистый класс 3: Датчик освещенности
    public class LightSensor : Sensor
    {
        public double LuxRange { get; set; }
        private double _luxValue;

        public LightSensor(string location, double luxRange, double luxValue) 
            : base(location)
        {
            LuxRange = luxRange;
            _luxValue = luxValue;
        }

        public override double GetReading()
        {
            Console.WriteLine($"[LightSensor] Локация: {Location} | Освещенность: {_luxValue} Люкс (Диапазон: {LuxRange} Люкс)");
            return _luxValue;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== 1. Создание коллекции базового типа List<Sensor> ===");
            List<Sensor> sensors = new List<Sensor>
            {
                new TemperatureSensor("Серверная", "C", 22.5),
                new PressureSensor("Цех №1", 200.0, 101.3),
                new LightSensor("Офис 304", 1000.0, 450.0),
                new TemperatureSensor("Лаборатория", "C", 19.8)
            };

            Console.WriteLine("\n=== 2. Полиморфный вызов метода GetReading() ===");
            List<double> readings = new List<double>();

            foreach (var sensor in sensors)
            {
                // Динамическое связывание сработает для каждого типа индивидуально
                double reading = sensor.GetReading();
                readings.Add(reading);
            }

            Console.WriteLine("\n=== 3. Агрегация результатов ===");
            double averageReading = readings.Average();
            double maxReading = readings.Max();
            double minReading = readings.Min();

            Console.WriteLine($"Всего датчиков опрошено: {readings.Count}");
            Console.WriteLine($"Среднее значение показателей: {averageReading:F2}");
            Console.WriteLine($"Максимальное значение: {maxReading:F2}");
            Console.WriteLine($"Минимальное значение: {minReading:F2}");
        }
    }
}
