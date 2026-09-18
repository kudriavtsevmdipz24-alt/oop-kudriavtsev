using System;

class Library
{
    private string _name;
    private string _address;
    private int _booksCount;

    public string Name
    {
        get { return _name; }
    }

    public string Address
    {
        get { return _address; }
    }

    public int BooksCount
    {
        get { return _booksCount; }
    }

    public Library() : this("Unknown", "Unknown", 0)
    {
    }

    public Library(string name, string address, int initialBooksCount)
    {
        if (initialBooksCount < 0)
            throw new ArgumentException("Кількість книг не може бути від'ємною.");

        _name = name;
        _address = address;
        _booksCount = initialBooksCount;

        Console.WriteLine($"Створено бібліотеку: {_name}");
    }

    public void AddBook(string bookTitle)
    {
        _booksCount++;
        Console.WriteLine($"До бібліотеки \"{_name}\" додано книгу: {bookTitle}");
        Console.WriteLine($"Кількість книг: {_booksCount}");
    }

    ~Library()
    {
        Console.WriteLine($"Знищується об'єкт бібліотеки: {_name}");
    }
}

class Program
{
    static void Main()
    {
        Console.WriteLine("=== Створення об'єктів ===");

        Library library1 = new Library();
        Library library2 = new Library("Міська бібліотека", "вул. Соборна, 10", 100);
        Library library3 = new Library("Бібліотека коледжу", "вул. Київська, 15", 50);

        Console.WriteLine();

        library1.AddBook("Основи C#");
        library2.AddBook("Об'єктно-орієнтоване програмування");
        library3.AddBook("Алгоритми та структури даних");

        Console.WriteLine();
        Console.WriteLine($"Бібліотека 1: {library1.Name}, книг: {library1.BooksCount}");
        Console.WriteLine($"Бібліотека 2: {library2.Name}, книг: {library2.BooksCount}");
        Console.WriteLine($"Бібліотека 3: {library3.Name}, книг: {library3.BooksCount}");

        Console.WriteLine();
        Console.WriteLine("=== Завершення Main ===");

        library1 = null;
        library2 = null;
        library3 = null;

        GC.Collect();
        GC.WaitForPendingFinalizers();

        Console.WriteLine("=== GC завершено ===");
    }
}