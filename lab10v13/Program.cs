using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace lab10v13
{
    // 1. Сутності даних (Варіант 13)
    public class Recipe
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Cuisine { get; set; }
        public int PrepTimeMinutes { get; set; }

        public Recipe(int id, string name, string cuisine, int prepTimeMinutes)
        {
            Id = id;
            Name = name;
            Cuisine = cuisine;
            PrepTimeMinutes = prepTimeMinutes;
        }
    }

    public class Ingredient
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int RecipeId { get; set; }

        public Ingredient(int id, string name, int recipeId)
        {
            Id = id;
            Name = name;
            RecipeId = recipeId;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Лабораторна робота №10: LINQ у дії (Варіант 13: Управління рецептами) ===\n");

            // 2. Генерація тестових даних (понад 10-15 елементів)
            List<Recipe> recipes = new List<Recipe>
            {
                new Recipe(1, "Паста Карбонара", "Italian", 25),
                new Recipe(2, "Піца Маргарита", "Italian", 20),
                new Recipe(3, "Різотто з грибами", "Italian", 45),
                new Recipe(4, "Борщ Український", "Ukrainian", 90),
                new Recipe(5, "Вареники з картоплею", "Ukrainian", 60),
                new Recipe(6, "Суші Каліфорнія", "Japanese", 40),
                new Recipe(7, "Рамен", "Japanese", 50),
                new Recipe(8, "Тако з яловичиною", "Mexican", 30),
                new Recipe(9, "Лазанья", "Italian", 75),
                new Recipe(10, "Гаспачо", "Spanish", 15),
                new Recipe(11, "Паста Болоньєзе", "Italian", 28),
                new Recipe(12, "Тірамісу", "Italian", 30)
            };

            List<Ingredient> ingredients = new Ingredient[]
            {
                new Ingredient(101, "Спагеті", 1),
                new Ingredient(102, "Панчета", 1),
                new Ingredient(103, "Яйця", 1),
                new Ingredient(104, "Сир Пармезан", 1),
                new Ingredient(105, "Тісто для піци", 2),
                new Ingredient(106, "Соус Томатний", 2),
                new Ingredient(107, "Моцарела", 2),
                new Ingredient(108, "Базилік", 2),
                new Ingredient(109, "Рис Арборіо", 3),
                new Ingredient(110, "Печериці", 3),
                new Ingredient(111, "Бульйон", 3),
                new Ingredient(112, "Буряк", 4),
                new Ingredient(113, "Капуста", 4),
                new Ingredient(114, "М'ясо", 4),
                new Ingredient(115, "Борошно", 5),
                new Ingredient(116, "Картопля", 5),
                new Ingredient(117, "Рис для суші", 6),
                new Ingredient(118, "Нопрі", 6),
                new Ingredient(119, "Авокадо", 6),
                new Ingredient(120, "Крабові палички", 6),
                new Ingredient(121, "Тортілья", 8),
                new Ingredient(122, "Яловичина фарш", 8)
            }.ToList();

            Stopwatch sw = new Stopwatch();

            // -----------------------------------------------------------------
            // Запит 1: Італійські рецепти з часом приготування до 30 хв
            // -----------------------------------------------------------------
            Console.WriteLine("--- 1. Фільтрація: Рецепти кухні 'Italian' із часом <= 30 хв ---");
            sw.Restart();
            
            // Query Syntax
            var query1 = from r in recipes
                         where r.Cuisine == "Italian" && r.PrepTimeMinutes <= 30
                         select r;
            var list1 = query1.ToList();
            sw.Stop();
            long querySyntaxTime1 = sw.ElapsedTicks;

            sw.Restart();
            // Method Syntax
            var method1 = recipes.Where(r => r.Cuisine == "Italian" && r.PrepTimeMinutes <= 30).ToList();
            sw.Stop();
            long methodSyntaxTime1 = sw.ElapsedTicks;

            foreach (var r in method1)
            {
                Console.WriteLine($" -> {r.Name} ({r.PrepTimeMinutes} хв)");
            }
            Console.WriteLine($"[Час виконання] Query Syntax: {querySyntaxTime1} ticks | Method Syntax: {methodSyntaxTime1} ticks\n");

            // -----------------------------------------------------------------
            // Запит 2: Join рецептів з їхніми інгредієнтами
            // -----------------------------------------------------------------
            Console.WriteLine("--- 2. Join: Об'єднання рецептів із їхніми інгредієнтами ---");
            sw.Restart();

            // Query Syntax
            var query2 = from r in recipes
                         join i in ingredients on r.Id equals i.RecipeId
                         select new { RecipeName = r.Name, IngredientName = i.Name };
            var list2 = query2.ToList();
            sw.Stop();
            long querySyntaxTime2 = sw.ElapsedTicks;

            sw.Restart();
            // Method Syntax
            var method2 = recipes.Join(ingredients,
                                       r => r.Id,
                                       i => i.RecipeId,
                                       (r, i) => new { RecipeName = r.Name, IngredientName = i.Name }).ToList();
            sw.Stop();
            long methodSyntaxTime2 = sw.ElapsedTicks;

            foreach (var item in method2.Take(5)) // покажемо перші 5 для лаконічності
            {
                Console.WriteLine($" -> Рецепт: '{item.RecipeName}' | Інгредієнт: '{item.IngredientName}'");
            }
            Console.WriteLine($" ... (всього об'єднано {method2.Count} пар)");
            Console.WriteLine($"[Час виконання] Query Syntax: {querySyntaxTime2} ticks | Method Syntax: {methodSyntaxTime2} ticks\n");

            // -----------------------------------------------------------------
            // Запит 3: Підрахунок кількості інгредієнтів для кожного рецепту
            // -----------------------------------------------------------------
            Console.WriteLine("--- 3. GroupJoin / Агрегація: Кількість інгредієнтів для кожного рецепту ---");
            sw.Restart();

            // Query Syntax
            var query3 = from r in recipes
                         join i in ingredients on r.Id equals i.RecipeId into ingGroup
                         select new { RecipeName = r.Name, Count = ingGroup.Count() };
            var list3 = query3.ToList();
            sw.Stop();
            long querySyntaxTime3 = sw.ElapsedTicks;

            sw.Restart();
            // Method Syntax
            var method3 = recipes.GroupJoin(ingredients,
                                            r => r.Id,
                                            i => i.RecipeId,
                                            (r, ingGroup) => new { RecipeName = r.Name, Count = ingGroup.Count() }).ToList();
            sw.Stop();
            long methodSyntaxTime3 = sw.ElapsedTicks;

            foreach (var item in method3)
            {
                Console.WriteLine($" -> Рецепт '{item.RecipeName}': {item.Count} інгредієнт(ів)");
            }
            Console.WriteLine($"[Час виконання] Query Syntax: {querySyntaxTime3} ticks | Method Syntax: {methodSyntaxTime3} ticks\n");

            // -----------------------------------------------------------------
            // Запит 4: Знайти рецепт з найдовшим часом приготування
            // -----------------------------------------------------------------
            Console.WriteLine("--- 4. Агрегація: Рецепт з найдовшим часом приготування ---");
            sw.Restart();

            // Query Syntax
            int maxTimeQuery = (from r in recipes select r.PrepTimeMinutes).Max();
            var longestRecipeQuery = (from r in recipes where r.PrepTimeMinutes == maxTimeQuery select r).FirstOrDefault();
            sw.Stop();
            long querySyntaxTime4 = sw.ElapsedTicks;

            sw.Restart();
            // Method Syntax
            var longestRecipeMethod = recipes.OrderByDescending(r => r.PrepTimeMinutes).FirstOrDefault();
            sw.Stop();
            long methodSyntaxTime4 = sw.ElapsedTicks;

            if (longestRecipeMethod != null)
            {
                Console.WriteLine($" -> Найдовший рецепт: '{longestRecipeMethod.Name}' ({longestRecipeMethod.PrepTimeMinutes} хв)");
            }
            Console.WriteLine($"[Час виконання] Query Syntax: {querySyntaxTime4} ticks | Method Syntax: {methodSyntaxTime4} ticks\n");
        }
    }
}