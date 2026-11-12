using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace IndependentWork11
{
    public class Document
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public override string ToString() => $"[{Id}] {Title}";
    }

    public static class TextProcessor
    {
        private static readonly HashSet<string> StopWords = new(StringComparer.OrdinalIgnoreCase)
            { "a", "an", "the", "is", "are", "was", "were", "in", "on", "at", "to", "for", "of", "and", "or" };

        public static IEnumerable<string> Tokenize(string text) =>
            string.IsNullOrWhiteSpace(text)
                ? Enumerable.Empty<string>()
                : Regex.Matches(text.ToLowerInvariant(), @"[a-z0-9#]+").Select(m => m.Value);

        public static IEnumerable<string> RemoveStopWords(IEnumerable<string> words) =>
            words.Where(w => !StopWords.Contains(w));
    }

    public class InvertedIndex
    {
        private readonly Dictionary<string, HashSet<int>> _index = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<int, Document> _docs = new();

        public int DocumentCount => _docs.Count;
        public int UniqueWords => _index.Count;

        public void AddDocument(Document doc)
        {
            _docs[doc.Id] = doc;
            foreach (var word in TextProcessor.RemoveStopWords(TextProcessor.Tokenize(doc.Content)))
            {
                if (!_index.ContainsKey(word)) _index[word] = new HashSet<int>();
                _index[word].Add(doc.Id);
            }
        }

        public List<Document> SearchSingleWord(string word)
        {
            var token = TextProcessor.Tokenize(word).FirstOrDefault();
            return token != null && _index.TryGetValue(token, out var ids)
                ? ids.Select(id => _docs[id]).ToList()
                : new List<Document>();
        }

        public List<Document> SearchMultipleWords(params string[] words)
        {
            var tokens = words.SelectMany(w => TextProcessor.RemoveStopWords(TextProcessor.Tokenize(w))).Distinct().ToList();
            if (tokens.Count == 0 || tokens.Any(t => !_index.ContainsKey(t))) return new List<Document>();

            var result = new HashSet<int>(_index[tokens[0]]);
            foreach (var t in tokens.Skip(1)) result.IntersectWith(_index[t]);
            return result.OrderBy(id => id).Select(id => _docs[id]).ToList();
        }

        public List<Document> SearchAnyWord(params string[] words)
        {
            var result = new HashSet<int>();
            foreach (var t in words.SelectMany(w => TextProcessor.RemoveStopWords(TextProcessor.Tokenize(w))))
                if (_index.TryGetValue(t, out var ids)) result.UnionWith(ids);
            return result.OrderBy(id => id).Select(id => _docs[id]).ToList();
        }
    }

    class Program
    {
        static void Main()
        {
            var baseDocs = new List<Document>
            {
                new() { Id = 1, Title = "Introduction to C#", Content = "C# is a modern object-oriented programming language developed by Microsoft." },
                new() { Id = 2, Title = "Java Basics", Content = "Java is a popular programming language known for its portability and performance." },
                new() { Id = 3, Title = "Python Overview", Content = "Python is a high-level programming language with simple syntax." },
                new() { Id = 4, Title = "C# Advanced Features", Content = "C# supports LINQ, async/await, and many modern programming paradigms." },
                new() { Id = 5, Title = "Comparing Languages", Content = "C#, Java, and Python are among the most popular programming languages today." }
            };

            // 1. Бенчмарк продуктивності
            Console.WriteLine("=== Тест продуктивності (Linear vs Inverted Index) ===");
            foreach (int size in new[] { 10, 100, 1000, 10000 })
            {
                var docs = Enumerable.Range(1, size)
                    .Select(i => new Document { Id = i, Title = $"Doc {i}", Content = baseDocs[(i - 1) % 5].Content })
                    .ToList();

                var idx = new InvertedIndex();
                docs.ForEach(idx.AddDocument);

                var sw = Stopwatch.StartNew();
                var lin = docs.Where(d => TextProcessor.RemoveStopWords(TextProcessor.Tokenize(d.Content)).Contains("linq")).ToList();
                long linTicks = sw.ElapsedTicks;

                sw.Restart();
                var ind = idx.SearchSingleWord("linq");
                long indTicks = sw.ElapsedTicks;

                Console.WriteLine($"Docs: {size,-5} | Linear: {linTicks,-7} ticks | Index: {indTicks,-5} ticks | Found: {lin.Count}/{ind.Count}");
            }

            // 2. Демонстрація пошуку
            var index = new InvertedIndex();
            baseDocs.ForEach(index.AddDocument);
            Console.WriteLine($"\nLoaded {index.DocumentCount} documents | Unique words: {index.UniqueWords}");

            Console.WriteLine("\n--- AND пошук: 'programming language' ---");
            index.SearchMultipleWords("programming", "language").ForEach(Console.WriteLine);

            Console.WriteLine("\n--- OR пошук: 'c# python' ---");
            index.SearchAnyWord("c#", "python").ForEach(Console.WriteLine);

            // 3. Інтерактивний режим
            while (true)
            {
                Console.Write("\nEnter search query (or 'exit'): ");
                var q = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(q) || q == "exit") break;

                var found = index.SearchMultipleWords(q.Split(' ', StringSplitOptions.RemoveEmptyEntries));
                found.ForEach(d => Console.WriteLine($"{d} -> {d.Content}"));
                Console.WriteLine($"Found: {found.Count}");
            }
        }
    }
}