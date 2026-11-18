using System;
using System.Collections.Generic;
using System.Linq;

namespace IndependentWork14
{
    // Сценарій 1: Кеш метаданих (Dictionary)
    public class FileMetadata
    {
        public string FileName { get; set; } = "";
        public long Size { get; set; }
        public DateTime LastModified { get; set; }
        public string Hash { get; set; } = "";
        public override string ToString() => $"{FileName} ({Size} B, Hash: {Hash})";
    }

    public class FileMetadataCache
    {
        private readonly Dictionary<string, FileMetadata> _cache = new(StringComparer.OrdinalIgnoreCase);

        public void Add(FileMetadata meta) => _cache[meta.FileName] = meta;
        public FileMetadata? Get(string fileName) => _cache.TryGetValue(fileName, out var meta) ? meta : null;
        public bool Remove(string fileName) => _cache.Remove(fileName);
        public bool Contains(string fileName) => _cache.ContainsKey(fileName);
    }

    // Сценарій 2: Унікальні теги (HashSet)
    public class TagManager
    {
        private readonly Dictionary<string, HashSet<string>> _articleTags = new(StringComparer.OrdinalIgnoreCase);

        public void AddTag(string articleId, string tag)
        {
            if (!_articleTags.ContainsKey(articleId))
                _articleTags[articleId] = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            _articleTags[articleId].Add(tag);
        }

        public void RemoveTag(string articleId, string tag)
        {
            if (_articleTags.TryGetValue(articleId, out var tags))
                tags.Remove(tag);
        }

        public HashSet<string> GetAllUniqueTags()
        {
            var all = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var tags in _articleTags.Values)
                all.UnionWith(tags); // Об'єднання множин
            return all;
        }

        public HashSet<string> FindCommonTags(string id1, string id2)
        {
            if (!_articleTags.TryGetValue(id1, out var t1) || !_articleTags.TryGetValue(id2, out var t2))
                return new HashSet<string>();

            var common = new HashSet<string>(t1, StringComparer.OrdinalIgnoreCase);
            common.IntersectWith(t2); // Перетин множин
            return common;
        }

        public HashSet<string> FindUniqueTagsForFirst(string id1, string id2)
        {
            if (!_articleTags.TryGetValue(id1, out var t1)) return new HashSet<string>();
            var diff = new HashSet<string>(t1, StringComparer.OrdinalIgnoreCase);
            if (_articleTags.TryGetValue(id2, out var t2))
                diff.ExceptWith(t2); // Різниця множин
            return diff;
        }
    }

    // Сценарій 3 (додатково): Пошуковий індекс (Dictionary + HashSet)
    public class SearchIndex
    {
        private readonly Dictionary<string, HashSet<int>> _invertedIndex = new(StringComparer.OrdinalIgnoreCase);

        public void AddDocument(int docId, string content)
        {
            var words = content.Split(new[] { ' ', '.', ',', '!', '?', '-' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var w in words)
            {
                if (!_invertedIndex.ContainsKey(w))
                    _invertedIndex[w] = new HashSet<int>();
                _invertedIndex[w].Add(docId);
            }
        }

        public HashSet<int> Search(string word) =>
            _invertedIndex.TryGetValue(word, out var ids) ? new HashSet<int>(ids) : new HashSet<int>();

        public HashSet<int> SearchMultiple(params string[] words)
        {
            if (words.Length == 0) return new HashSet<int>();
            var result = Search(words[0]);
            foreach (var w in words.Skip(1))
                result.IntersectWith(Search(w));
            return result;
        }
    }

    class Program
    {
        static void Main()
        {
            Console.WriteLine("=== Самостійна робота №14: Dictionary та HashSet у .NET ===\n");

            // 1. Кеш метаданих
            Console.WriteLine("--- 1. FileMetadataCache (Dictionary) ---");
            var cache = new FileMetadataCache();
            cache.Add(new FileMetadata { FileName = "report.pdf", Size = 204800, LastModified = DateTime.Now, Hash = "9F86D081" });
            cache.Add(new FileMetadata { FileName = "config.json", Size = 1024, LastModified = DateTime.Now, Hash = "4B227777" });

            Console.WriteLine($"Пошук 'REPORT.PDF': {cache.Get("REPORT.PDF")}");
            Console.WriteLine($"Contains 'config.json': {cache.Contains("config.json")}\n");

            // 2. Унікальні теги
            Console.WriteLine("--- 2. TagManager (HashSet operations) ---");
            var tags = new TagManager();
            tags.AddTag("post1", "C#");
            tags.AddTag("post1", "Collections");
            tags.AddTag("post1", "Performance");

            tags.AddTag("post2", "c#"); // дублікат у іншому регістрі ігнорується
            tags.AddTag("post2", "LINQ");
            tags.AddTag("post2", "Collections");

            Console.WriteLine($"Усі унікальні теги (UnionWith): {string.Join(", ", tags.GetAllUniqueTags())}");
            Console.WriteLine($"Спільні теги post1 і post2 (IntersectWith): {string.Join(", ", tags.FindCommonTags("post1", "post2"))}");
            Console.WriteLine($"Теги лише у post1 (ExceptWith): {string.Join(", ", tags.FindUniqueTagsForFirst("post1", "post2"))}\n");

            // 3. Пошуковий індекс
            Console.WriteLine("--- 3. SearchIndex (Dictionary + HashSet) ---");
            var index = new SearchIndex();
            index.AddDocument(1, "Dictionary та HashSet забезпечують швидкий пошук O(1).");
            index.AddDocument(2, "HashSet зберігає лише унікальні елементи в C#.");
            index.AddDocument(3, "Dictionary використовує хеш-таблицю для ключів у C#.");

            Console.WriteLine($"Документи зі словом 'HashSet': [{string.Join(", ", index.Search("HashSet"))}]");
            Console.WriteLine($"Документи зі словами 'C#' та 'Dictionary': [{string.Join(", ", index.SearchMultiple("C#", "Dictionary"))}]");
        }
    }
}