using System;
using System.Collections.Generic;

namespace IndependentWork5
{
    // 1. Базовий клас LibraryItem
    public class LibraryItem
    {
        private readonly string _uniqueId;
        private string _title;
        private int _publicationYear;

        public string UniqueId => _uniqueId;
        public string Title => _title;
        public int PublicationYear => _publicationYear;

        // Конструктор базового класу з оновленим параметром uniqueId
        public LibraryItem(string uniqueId, string title, int publicationYear)
        {
            _uniqueId = uniqueId ?? throw new ArgumentNullException(nameof(uniqueId));
            _title = title;
            _publicationYear = publicationYear;
        }

        public virtual void DisplayDetails()
        {
            Console.WriteLine($"[ID: {UniqueId}] '{Title}' ({PublicationYear} р.)");
        }
    }

    // 2. Похідний клас Book (успадковує LibraryItem)
    public class Book : LibraryItem
    {
        private string _author;

        public string Author => _author;

        public Book(string uniqueId, string title, int publicationYear, string author)
            : base(uniqueId, title, publicationYear)
        {
            _author = author;
        }

        public override void DisplayDetails()
        {
            Console.WriteLine($"[Книга | ID: {UniqueId}] '{Title}' ({PublicationYear} р.) | Автор: {Author}");
        }
    }

    // 3. Похідний клас Journal (успадковує LibraryItem)
    public class Journal : LibraryItem
    {
        private int _issueNumber;

        public int IssueNumber => _issueNumber;

        public Journal(string uniqueId, string title, int publicationYear, int issueNumber)
            : base(uniqueId, title, publicationYear)
        {
            _issueNumber = issueNumber;
        }

        public override void DisplayDetails()
        {
            Console.WriteLine($"[Журнал | ID: {UniqueId}] '{Title}' ({PublicationYear} р.) | Випуск №{IssueNumber}");
        }
    }

    // 4. Похідний клас AudioBook (успадковує Book)
    public class AudioBook : Book
    {
        private string _narrator;

        public string Narrator => _narrator;

        public AudioBook(string uniqueId, string title, int publicationYear, string author, string narrator)
            : base(uniqueId, title, publicationYear, author)
        {
            _narrator = narrator;
        }

        public override void DisplayDetails()
        {
            Console.WriteLine($"[Аудіокнига | ID: {UniqueId}] '{Title}' ({PublicationYear} р.) | Автор: {Author} | Оповідач: {Narrator}");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Самостійна робота №5: Розширення ієрархії та наслідки змін базового класу ===\n");

            // Створення об'єктів для кожної сутності ієрархії
            List<LibraryItem> libraryItems = new List<LibraryItem>
            {
                new Book("LIB-001", "Кобзар", 1840, "Тарас Шевченко"),
                new Journal("LIB-002", "Наука та життя", 2023, 5),
                new AudioBook("LIB-003", "Тіні забутих предків", 1911, "Михайло Коцюбинський", "Олександр Власюк")
            };

            Console.WriteLine("--- Поліморфний вивід відомостей про бібліотечні елементи ---");
            foreach (var item in libraryItems)
            {
                item.DisplayDetails();
            }
        }
    }
}
