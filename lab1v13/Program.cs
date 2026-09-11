using System;

class Library
{
private string name;
private string address;


public int BooksCount { get; private set; }

public Library(string name, string address, int booksCount)
{
    this.name = name;
    this.address = address;
    BooksCount = booksCount;
}

public void AddBook()
{
    BooksCount++;
    Console.WriteLine($"До бібліотеки «{name}» додано книгу. Кількість книг: {BooksCount}");
}

public void GetInfo()
{
    Console.WriteLine($"Бібліотека: {name}");
    Console.WriteLine($"Адреса: {address}");
    Console.WriteLine($"Кількість книг: {BooksCount}");
}

~Library()
{
    Console.WriteLine($"Об'єкт бібліотеки «{name}» знищено.");
}

}

class Program
{
static void Main()
{
Library library1 = new Library("Центральна бібліотека", "м. Рівне, вул. Соборна", 1500);
Library library2 = new Library("Бібліотека коледжу", "м. Рівне, вул. Лермонтова", 800);
Library library3 = new Library("Міська бібліотека", "м. Рівне, вул. Київська", 1200);

    library1.GetInfo();
    library1.AddBook();

    Console.WriteLine();

    library2.GetInfo();
    library2.AddBook();

    Console.WriteLine();

    library3.GetInfo();
    library3.AddBook();
}


}

