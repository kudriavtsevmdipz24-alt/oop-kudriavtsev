using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace IndependentWork16
{
    public class Document
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Content { get; set; } = "";
        public override string ToString() => $"[{Id}] {Title}";
    }

    public static class TextProcessor
    {
        private static readonly HashSet<string> StopWords = new(StringComparer.OrdinalIgnoreCase)
        {
            "a", "an", "the", "is", "are", "was", "were",
            "in", "on", "at", "to", "for", "of", "and", "or"
        };

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
        private readonly Dictionary<int, Document> _documents = new();

        public int DocumentCount => _documents.Count;
        public int UniqueWords => _index.Keys.Count;

        public void AddDocument(Document doc)
        {
            _documents[doc.Id] = doc;
            var tokens = TextProcessor.RemoveStopWords(TextProcessor.Tokenize(doc.Content));

            foreach (var word in tokens)
            {
                if (!_index.TryGetValue(word, out var ids))
                {
                    ids = new HashSet<int>();
                    _index[word] = ids;
                }
                ids.Add(doc.Id);
            }
        }

        public List<Document> SearchSingleWord(string word)
        {
            var token = TextProcessor.RemoveStopWords(TextProcessor.Tokenize(word)).FirstOrDefault();
            return token != null && _index.TryGetValue(token, out var ids)
                ? ids.OrderBy(id => id).Select(id => _documents[id]).ToList()
                : new List<Document>();
        }

        public List<Document> SearchMultipleWords(params string[] words)
        {
            var tokens = words
                .SelectMany(w => TextProcessor.RemoveStopWords(TextProcessor.Tokenize(w)))
                .Distinct()
                .ToList();

            if (tokens.Count == 0 || tokens.Any(t => !_index.ContainsKey(t)))
                return new List<Document>();

            var result = new HashSet<int>(_index[tokens[0]]);
            foreach (var token in tokens.Skip(1))
                result.IntersectWith(_index[token]);

            return result.OrderBy(id => id).Select(id => _documents[id]).ToList();
        }

        public List<Document> SearchAnyWord(params string[] words)
        {
            var result = new HashSet<int>();
            var tokens = words.SelectMany(w => TextProcessor.RemoveStopWords(TextProcessor.Tokenize(w)));

            foreach (var token in tokens)
                if (_index.TryGetValue(token, out var ids))
                    result.UnionWith(ids);

            return result.OrderBy(id => id).Select(id => _documents[id]).ToList();
        }
    }

    public static class PerformanceTest
    {
        public static void CompareSearchMethods(List<Document> documents, string searchWord)
        {
            var sw = new Stopwatch();
            var normalizedWord = TextProcessor.Tokenize(searchWord).First();

            // 1. Лінійний пошук (з ідентичною нормалізацією)
            sw.Start();
            var linearResults = documents
                .Where(d => TextProcessor.RemoveStopWords(TextProcessor.Tokenize(d.Content)).Contains(normalizedWord))
                .ToList();
            sw.Stop();
            long linearTicks = sw.ElapsedTicks;

            // 2. Побудова та пошук в Inverted Index
            var index = new InvertedIndex();
            foreach (var doc in documents)
                index.AddDocument(doc);

            sw.Restart();
            var indexResults = index.SearchSingleWord(searchWord);
            sw.Stop();
            long indexTicks = sw.ElapsedTicks;

            Console.WriteLine($"Docs: {documents.Count,-6} | Linear: {linearTicks,-8} ticks | Index: {indexTicks,-6} ticks | Found: {linearResults.Count} == {indexResults.Count}");
        }
    }

    class Program
    {
        static List<Document> GetSampleDocuments() => new()
        {
            new() { Id = 1, Title = "Introduction to C#", Content = "C# is a modern object-oriented programming language developed by Microsoft." },
            new() { Id = 2, Title = "Java Basics", Content = "Java is a popular programming language known for its portability and performance." },
            new() { Id = 3, Title = "Python Overview", Content = "Python is a high-level programming language with simple syntax." },
            new() { Id = 4, Title = "C# Advanced Features", Content = "C# supports LINQ, async/await, and many modern programming paradigms." },
            new() { Id = 5, Title = "Comparing Languages", Content = "C#, Java, and Python are among the most popular programming languages today." }
        };

        static void Main()
        {
            Console.WriteLine("=== Самостійна робота №16: Inverted Index та вимірювання продуктивності ===\n");

            // 1. Вимірювання продуктивності (10, 100, 1000, 10000 документів)
            Console.WriteLine("--- Частина 3: Порівняння продуктивності (слово 'linq') ---");
            var baseDocs = GetSampleDocuments();
            foreach (int count in new[] { 10, 100, 1000, 10000 })
            {
                var generated = Enumerable.Range(1, count)
                    .Select(i => new Document
                    {
                        Id = i,
                        Title = $"Doc #{i}",
                        Content = baseDocs[(i - 1) % baseDocs.Count].Content
                    })
                    .ToList();

                PerformanceTest.CompareSearchMethods(generated, "linq");
            }

            // 2. Демонстрація пошуку на 5 базових документах
            var index = new InvertedIndex();
            baseDocs.ForEach(index.AddDocument);

            Console.WriteLine($"\nLoaded {index.DocumentCount} documents");
            Console.WriteLine($"Index contains {index.UniqueWords} unique words\n");

            // Демонстраційні запити AND та OR
            PrintResults("programming language (AND)", index.SearchMultipleWords("programming", "language"));
            PrintResults("c# async (AND)", index.SearchMultipleWords("c#", "async"));
            PrintResults("java python (OR)", index.SearchAnyWord("java", "python"));

            // Інтерактивний режим
            while (true)
            {
                Console.Write("Enter search query (or 'exit'): ");
                var query = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(query) || query.Trim().Equals("exit", StringComparison.OrdinalIgnoreCase))
                    break;

                var words = query.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                PrintResults($"{query} (AND)", index.SearchMultipleWords(words));
            }
        }

        static void PrintResults(string label, List<Document> results)
        {
            Console.WriteLine($"--- Search Results: {label} ---");
            foreach (var doc in results)
            {
                Console.WriteLine(doc);
                Console.WriteLine($"  {doc.Content.Substring(0, Math.Min(85, doc.Content.Length))}...");
            }
            Console.WriteLine($"Found {results.Count} documents\n");
        }
    }
}
