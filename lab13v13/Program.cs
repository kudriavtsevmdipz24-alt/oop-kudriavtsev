using System;

namespace lab13v13
{
    // 1. Власний виняток для Варіанту 13
    public class InvalidInputException : Exception
    {
        public string FieldName { get; }
        public string Reason { get; }

        public InvalidInputException() { }

        public InvalidInputException(string message) : base(message) { }

        public InvalidInputException(string message, Exception innerException)
            : base(message, innerException) { }

        public InvalidInputException(string fieldName, string reason)
            : base($"Помилка валідації поля '{fieldName}': {reason}")
        {
            FieldName = fieldName;
            Reason = reason;
        }
    }

    // 2. Узагальнений клас патерну Result<T>
    public class Result<T>
    {
        public bool IsSuccess { get; }
        public T Value { get; }
        public string Error { get; }

        private Result(T value, bool isSuccess, string error)
        {
            Value = value;
            IsSuccess = isSuccess;
            Error = error;
        }

        public static Result<T> Success(T value) => new(value, true, null);
        public static Result<T> Failure(string error) => new(default, false, error);
        public static Result<T> Failure(Exception ex) => new(default, false, ex?.Message ?? "Невідома помилка");
    }

    // 3. Сервіс обробки вхідних даних (Варіант 13)
    public class InputProcessor
    {
        // Метод 1: Використовує власний виняток при некоректному перетворенні в число
        public int ParseInt(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                throw new InvalidInputException(nameof(input), "Вхідний рядок не може бути порожнім.");

            if (!int.TryParse(input, out int result))
                throw new InvalidInputException(nameof(input), $"Значення '{input}' не є цілим числом.");

            return result;
        }

        // Метод 2: Використовує патерн Result<T> для очікуваних перевірок довжини рядка
        public Result<bool> ValidateString(string input, int minLength)
        {
            if (minLength < 0)
                return Result<bool>.Failure(new ArgumentOutOfRangeException(nameof(minLength), "Мінімальна довжина не може бути від'ємною."));

            if (string.IsNullOrEmpty(input))
                return Result<bool>.Failure("Рядок не може бути порожнім або null.");

            if (input.Length < minLength)
                return Result<bool>.Failure($"Довжина рядка ({input.Length}) менша за мінімально дозволену ({minLength}).");

            return Result<bool>.Success(true);
        }
    }

    class Program
    {
        static void Main()
        {
            Console.WriteLine("=== Лабораторна робота №13: Власні винятки та патерн Result ===");
            Console.WriteLine("=== Варіант 13: Обробка вхідних даних ===\n");

            var processor = new InputProcessor();

            // -----------------------------------------------------------------
            // 1. Демонстрація ParseInt (Власний виняток InvalidInputException)
            // -----------------------------------------------------------------
            Console.WriteLine("--- 1. Тестування ParseInt (try-catch + InvalidInputException) ---");
            foreach (var testInput in new[] { "42", "abc123", "" })
            {
                try
                {
                    int number = processor.ParseInt(testInput);
                    Console.WriteLine($"[Успіх] Рядок '{testInput}' -> число {number}");
                }
                catch (InvalidInputException ex)
                {
                    Console.WriteLine($"[Виняток] Поле: {ex.FieldName} | Причина: {ex.Reason}");
                }
            }

            // -----------------------------------------------------------------
            // 2. Демонстрація ValidateString (Патерн Result<bool>)
            // -----------------------------------------------------------------
            Console.WriteLine("\n--- 2. Тестування ValidateString (патерн Result<bool>) ---");
            var stringTests = new[]
            {
                (Text: "Kudriavtsev", MinLen: 5),
                (Text: "OOP", MinLen: 5),
                (Text: "", MinLen: 3)
            };

            foreach (var test in stringTests)
            {
                Result<bool> result = processor.ValidateString(test.Text, test.MinLen);

                if (result.IsSuccess)
                    Console.WriteLine($"[Result.Success] Рядок '{test.Text}' пройшов перевірку (мінімум {test.MinLen} симв.).");
                else
                    Console.WriteLine($"[Result.Failure] Рядок '{test.Text}' -> Помилка: {result.Error}");
            }
        }
    }
}
