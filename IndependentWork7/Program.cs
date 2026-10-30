using System;
using System.Collections.Generic;

namespace IndependentWork7
{
    // =========================================================================
    // СЦЕНАРІЙ 1: Система сповіщень (Інтерфейс INotifier)
    // =========================================================================

    public interface INotifier
    {
        void Send(string message);
    }

    public class EmailNotifier : INotifier
    {
        public void Send(string message)
        {
            Console.WriteLine($"[Email-сповіщення]: Надсилання на пошту: '{message}'");
        }
    }

    public class SmsNotifier : INotifier
    {
        public void Send(string message)
        {
            Console.WriteLine($"[SMS-сповіщення]: Надсилання SMS: '{message}'");
        }
    }

    public class PushNotifier : INotifier
    {
        public void Send(string message)
        {
            Console.WriteLine($"[Push-сповіщення]: Надсилання push-сповіщення: '{message}'");
        }
    }

    // =========================================================================
    // СЦЕНАРІЙ 2: Обробка файлів (Абстрактний клас FileProcessor)
    // =========================================================================

    public abstract class FileProcessor
    {
        public string FilePath { get; }

        protected FileProcessor(string filePath)
        {
            FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
        }

        // Спільний метод (Шаблонний метод), що реалізує єдиний алгоритм
        public void Process()
        {
            OpenFile();
            ProcessContent();
            CloseFile();
        }

        private void OpenFile()
        {
            Console.WriteLine($"\n[Відкриття файлу]: Файл '{FilePath}' успішно відкрито.");
        }

        private void CloseFile()
        {
            Console.WriteLine($"[Закриття файлу]: Файл '{FilePath}' закрито, ресурси звільнено.");
        }

        // Абстрактний метод, який повинен реалізувати кожен конкретний обробник
        protected abstract void ProcessContent();
    }

    public class TextFileProcessor : FileProcessor
    {
        public TextFileProcessor(string filePath) : base(filePath) { }

        protected override void ProcessContent()
        {
            Console.WriteLine($"[Обробка Text]: Читання та аналіз рядків текстового файлу '{FilePath}'...");
        }
    }

    public class XmlFileProcessor : FileProcessor
    {
        public XmlFileProcessor(string filePath) : base(filePath) { }

        protected override void ProcessContent()
        {
            Console.WriteLine($"[Обробка XML]: Валідація схеми та парсинг XML-дерева файлу '{FilePath}'...");
        }
    }

    // =========================================================================
    // Головний клас програми
    // =========================================================================

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Самостійна робота №7: Інтерфейс vs абстрактний клас ===\n");

            // -----------------------------------------------------------------
            // Демонстрація Сценарію 1 (Інтерфейс INotifier)
            // -----------------------------------------------------------------
            Console.WriteLine("--- 1. Демонстрація Сценарію 1: Система сповіщень (Інтерфейси) ---");
            List<INotifier> notifiers = new List<INotifier>
            {
                new EmailNotifier(),
                new SmsNotifier(),
                new PushNotifier()
            };

            foreach (INotifier notifier in notifiers)
            {
                notifier.Send("Важливе системне сповіщення!");
            }

            // -----------------------------------------------------------------
            // Демонстрація Сценарію 2 (Абстрактний клас FileProcessor)
            // -----------------------------------------------------------------
            Console.WriteLine("\n-----------------------------------------------------------------");
            Console.WriteLine("--- 2. Демонстрація Сценарію 2: Обробка файлів (Абстрактні класи) ---");
            List<FileProcessor> processors = new List<FileProcessor>
            {
                new TextFileProcessor("document.txt"),
                new XmlFileProcessor("config.xml")
            };

            foreach (FileProcessor processor in processors)
            {
                processor.Process();
            }
        }
    }
}
