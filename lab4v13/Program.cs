using System;

namespace lab4v13
{
    // Базовий клас
    public class Furniture
    {
        public string Material { get; set; }
        public double Weight { get; set; }

        public Furniture(string material, double weight)
        {
            Material = material;
            Weight = weight;
        }

        // Віртуальний метод для перевизначення
        public virtual void Assemble()
        {
            Console.WriteLine($"[Furniture] Збирання базових меблів із матеріалу {Material} (Вага: {Weight} кг).");
        }

        // Базовий метод для демонстрації приховування через 'new'
        public string GetFurnitureType()
        {
            return "Загальні меблі (Furniture)";
        }
    }

    // Похідний клас 1
    public class Chair : Furniture
    {
        public bool HasArmrests { get; set; }

        // Конструктор з викликом base(...)
        public Chair(string material, double weight, bool hasArmrests) 
            : base(material, weight)
        {
            HasArmrests = hasArmrests;
        }

        // Перевизначення (override) віртуального методу
        public override void Assemble()
        {
            string armrestsInfo = HasArmrests ? "із підлокітниками" : "без підлокітників";
            Console.WriteLine($"[Chair] Складання крісла з {Material} {armrestsInfo}. Вага: {Weight} кг.");
        }

        // Власний унікальний метод
        public void SitOn()
        {
            Console.WriteLine($"[Chair] Ви сіли на зручне крісло з матеріалу {Material}.");
        }

        // Демонстрація new (приховування члена базового класу)
        public new string GetFurnitureType()
        {
            return "Крісло (Chair)";
        }
    }

    // Похідний клас 2
    public class Table : Furniture
    {
        public int NumLegs { get; set; }

        // Конструктор з викликом base(...)
        public Table(string material, double weight, int numLegs) 
            : base(material, weight)
        {
            NumLegs = numLegs;
        }

        // Перевизначення (override) віртуального методу
        public override void Assemble()
        {
            Console.WriteLine($"[Table] Монтаж столу на {NumLegs} ніжках із матеріалу {Material}. Вага: {Weight} кг.");
        }

        // Власний унікальний метод
        public void PlaceItems()
        {
            Console.WriteLine($"[Table] На стіл розміщено предмети.");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== 1. Створення об'єктів та виклик власних методів ===");
            Chair myChair = new Chair("Дерево", 7.5, true);
            Table myTable = new Table("Скло", 18.0, 4);

            myChair.SitOn();
            myTable.PlaceItems();

            Console.WriteLine("\n=== 2. Демонстрація поліморфізму (override) ===");
            Furniture[] inventory = new Furniture[]
            {
                new Furniture("Пластик", 3.0),
                myChair,
                myTable
            };

            foreach (var item in inventory)
            {
                item.Assemble();
            }

            Console.WriteLine("\n=== 3. Демонстрація різниці між override та new ===");
            Console.WriteLine($"Виклик через посилання Chair: {myChair.GetFurnitureType()}");

            Furniture chairAsFurniture = myChair;
            Console.WriteLine($"Виклик через посилання Furniture (для того ж об'єкта): {chairAsFurniture.GetFurnitureType()}");
        }
    }
}
