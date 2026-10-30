using System;
using System.Collections.Generic;

namespace lab9v13
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Лабораторна робота №9: Практика з HashSet, Queue, Stack ===");
            Console.WriteLine("=== Варіант 13: Система чату ===\n");

            // -------------------------------------------------------------
            // 1. HashSet<T>: Унікальні користувачі онлайн
            // -------------------------------------------------------------
            Console.WriteLine("--- 1. HashSet<T>: Управління користувачами онлайн ---");
            HashSet<string> onlineUsers = new HashSet<string>();

            // Додавання користувачів (включно з дублікатами)
            Console.WriteLine($"Додаємо 'Олександр': {onlineUsers.Add("Олександр")}");
            Console.WriteLine($"Додаємо 'Марія': {onlineUsers.Add("Марія")}");
            Console.WriteLine($"Додаємо 'Максим': {onlineUsers.Add("Максим")}");
            Console.WriteLine($"Спроба повторно додати 'Олександр': {onlineUsers.Add("Олександр")}"); // Поверне false

            Console.WriteLine($"\nКількість унікальних користувачів онлайн: {onlineUsers.Count}");
            Console.WriteLine($"Перевірка наявності 'Марія' (Contains): {onlineUsers.Contains("Марія")}");

            Console.WriteLine("Список користувачів у мережі:");
            foreach (var user in onlineUsers)
            {
                Console.WriteLine($" - {user}");
            }

            // -------------------------------------------------------------
            // 2. Queue<T>: Черга повідомлень (FIFO - First In, First Out)
            // -------------------------------------------------------------
            Console.WriteLine("\n--------------------------------------------------");
            Console.WriteLine("--- 2. Queue<T>: Черга відправки повідомлень (FIFO) ---");
            Queue<string> messageQueue = new Queue<string>();

            // Додавання повідомлень у чергу
            messageQueue.Enqueue("Олександр: Привіт усім!");
            messageQueue.Enqueue("Марія: Доброго дня, як справи?");
            messageQueue.Enqueue("Максим: Усім привіт, працюємо над лабою.");

            Console.WriteLine($"Наступне повідомлення на обробку (Peek): '{messageQueue.Peek()}'");
            Console.WriteLine($"Всього повідомлень у черзі: {messageQueue.Count}\n");

            Console.WriteLine("Обробка та надсилання повідомлень за порядком надходження:");
            while (messageQueue.Count > 0)
            {
                string msg = messageQueue.Dequeue();
                Console.WriteLine($"[Надіслано]: {msg}");
            }

            // -------------------------------------------------------------
            // 3. Stack<T>: Історія відкритих повідомлень (LIFO - Last In, First Out)
            // -------------------------------------------------------------
            Console.WriteLine("\n--------------------------------------------------");
            Console.WriteLine("--- 3. Stack<T>: Історія перегляду повідомлень (LIFO) ---");
            Stack<string> messageHistory = new Stack<string>();

            // Додавання повідомлень до історії
            messageHistory.Push("Повідомлення #1 від Олександра");
            messageHistory.Push("Повідомлення #2 від Марії");
            messageHistory.Push("Повідомлення #3 від Максима");

            Console.WriteLine($"Останнє переглянуте повідомлення (Peek): '{messageHistory.Peek()}'");
            Console.WriteLine($"Всього повідомлень в історії: {messageHistory.Count}\n");

            Console.WriteLine("Скасування / Повернення назад по історії (Undo):");
            while (messageHistory.Count > 0)
            {
                string lastMsg = messageHistory.Pop();
                Console.WriteLine($"[Видалено з історії / Закрито]: {lastMsg}");
            }
        }
    }
}
