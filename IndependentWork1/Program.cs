using System;

namespace IndependentWork1
{
    // Клас 1: Смартфон (моделювання електронного пристрою)
    public class Smartphone
    {
        // Приватні поля
        private string _model;
        private double _batteryLevel;

        // Властивість (read-only)
        public string Model => _model;

        // Властивість get/set
        public double BatteryLevel
        {
            get { return _batteryLevel; }
            set { _batteryLevel = Math.Clamp(value, 0, 100); }
        }

        // Конструктор
        public Smartphone(string model, double batteryLevel)
        {
            _model = model;
            BatteryLevel = batteryLevel;
        }

        // Метод з обчисленням/перевіркою стану
        public void ChargeBattery(double amount)
        {
            BatteryLevel += amount;
            Console.WriteLine($"[Smartphone] Пристрій {_model} заряджено. Поточний рівень: {BatteryLevel}%");
        }
    }

    // Клас 2: Навчальний курс
    public class Course
    {
        private string _title;
        private int _hoursCount;

        public string Title
        {
            get { return _title; }
            private set { _title = value; }
        }

        public int HoursCount => _hoursCount;

        public Course(string title, int hoursCount)
        {
            _title = title;
            _hoursCount = hoursCount;
        }

        // Метод для розрахунку кількості тижнів навчання
        public double CalculateWeeksCount(int hoursPerWeek)
        {
            if (hoursPerWeek <= 0) return 0;
            return (double)_hoursCount / hoursPerWeek;
        }
    }

    // Клас 3: Баланс рахунку (числове поле + перевірка)
    public class AccountBalance
    {
        private string _accountOwner;
        private decimal _balance;

        public string AccountOwner => _accountOwner;

        public decimal Balance
        {
            get { return _balance; }
            private set { _balance = value; }
        }

        public AccountBalance(string accountOwner, decimal initialBalance)
        {
            _accountOwner = accountOwner;
            _balance = initialBalance;
        }

        // Метод поповнення балансу з поверненням нового значення
        public decimal Deposit(decimal amount)
        {
            if (amount > 0)
            {
                _balance += amount;
                Console.WriteLine($"[AccountBalance] Рахунок {_accountOwner} поповнено на {amount} грн.");
            }
            return _balance;
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("=== Самостійна робота №1: Демонстрація роботи класів ===\n");

            // 1. Демонстрація класу Smartphone
            Smartphone phone = new Smartphone("Samsung Galaxy S23", 45.0);
            Console.WriteLine($"Модель пристрою: {phone.Model}, Рівень заряду: {phone.BatteryLevel}%");
            phone.ChargeBattery(30.0);

            Console.WriteLine("\n--------------------------------------------------");

            // 2. Демонстрація класу Course
            Course oopCourse = new Course("Об'єктно-орієнтоване програмування", 80);
            double weeksNeeded = oopCourse.CalculateWeeksCount(10);
            Console.WriteLine($"Курс: '{oopCourse.Title}' ({oopCourse.HoursCount} год.)");
            Console.WriteLine($"Тривалість вивчення (при 10 год/тиждень): {weeksNeeded} тижнів.");

            Console.WriteLine("\n--------------------------------------------------");

            // 3. Демонстрація класу AccountBalance
            AccountBalance myAccount = new AccountBalance("Максим", 1500.50m);
            Console.WriteLine($"Власник рахунку: {myAccount.AccountOwner}, Початковий баланс: {myAccount.Balance} грн.");
            decimal currentBalance = myAccount.Deposit(500.00m);
            Console.WriteLine($"Оновлений баланс: {currentBalance} грн.");
        }
    }
}