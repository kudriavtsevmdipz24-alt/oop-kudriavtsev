using System;
using System.Collections.Generic;
using System.Linq;

namespace IndependentWork9
{
    // =========================================================================
    // Сценарій 1: Кеш метаданих (Dictionary)
    // =========================================================================
    public class FileMetadata
    {
        public string FileName { get; set; }
        public long Size { get; set; }
        public DateTime LastModified { get; set; }
        public string Hash { get; set; }

        public override string ToString()
        {
            return $"Файл: {FileName} | Розмір: {Size} байт | Хеш: {Hash}";
        }
    }

    public class FileMetadataCache
    {
        private readonly Dictionary<string, FileMetadata> _cache = new Dictionary<string, FileMetadata>(StringComparer.OrdinalIgnoreCase);

        public void Add(FileMetadata metadata)
        {
            if (metadata == null || string.IsNullOrWhiteSpace(metadata.FileName))
                return;

            _cache[metadata.FileName] = metadata;
        }

        public FileMetadata Get(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return null;
            _cache.TryGetValue(fileName, out var metadata);
            return metadata;
        }

        public bool Remove(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return false;
            return _cache.Remove(fileName);
        }

        public bool Contains(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return false;
            return _cache.ContainsKey(fileName);
        }

        public int Count => _cache.Count;
    }

    // =========================================================================
    // Сценарій 2: Унікальні теги (HashSet)
    // =========================================================================
    public class TagManager
    {
        private readonly Dictionary<string, HashSet<string>> _articleTags = new Dictionary<string, HashSet<string>>();

        public void AddTag(string articleId, string tag)
        {
            if (string.IsNullOrWhiteSpace(articleId) || string.IsNullOrWhiteSpace(tag)) return;

            if (!_articleTags.TryGetValue(articleId, out var tags))
            {
                tags = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                _articleTags[articleId] = tags;
            }
            tags.Add(tag);
        }

        public void RemoveTag(string articleId, string tag)
        {
            if (_articleTags.TryGetValue(articleId, out var tags))
            {
                tags.Remove(tag);
            }
        }

        public HashSet<string> GetTagsForArticle(string articleId)
        {
            if (_articleTags.TryGetValue(articleId, out var tags))
            {
                return new HashSet<string>(tags, StringComparer.OrdinalIgnoreCase);
            }
            return new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        }

        public HashSet<string> GetAllUniqueTags()
        {
            var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var tags in _articleTags.Values)
            {
                result.UnionWith(tags); // Множинна операція: об'єднання
            }
            return result;
        }

        public HashSet<string> FindCommonTags(string articleId1, string articleId2)
        {
            var tags1 = GetTagsForArticle(articleId1);
            var tags2 = GetTagsForArticle(articleId2);
            tags1.IntersectWith(tags2); // Множинна операція: перетин
            return tags1;
        }
    }

    // =========================================================================
    // Сценарій 3: Індекс для пошуку (Dictionary + HashSet)
    // =========================================================================
    public class SearchIndex
    {
        // Слово -> Множина ID документів, де воно зустрічається
        private readonly Dictionary<string, HashSet<int>> _invertedIndex = new Dictionary<string, HashSet<int>>(StringComparer.OrdinalIgnoreCase);

        public void AddDocument(int docId, string content)
        {
            if (string.IsNullOrWhiteSpace(content)) return;

            // Розбиваємо текст на слова та очищаємо від знаків пунктуації
            var words = content.Split(new[] { ' ', '.', ',', '!', '?', '-', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var word in words)
            {
                string cleanWord = word.Trim().ToLower();
                if (cleanWord.Length < 2) continue;

                if (!_invertedIndex.TryGetValue(cleanWord, out var docSet))
                {
                    docSet = new HashSet<int>();
                    _invertedIndex[cleanWord] = docSet;
                }
                docSet.Add(docId);
            }
        }

        public HashSet<int> Search(string word)
        {
            if (string.IsNullOrWhiteSpace(word)) return new HashSet<int>();

            if (_invertedIndex.TryGetValue(word.Trim().ToLower(), out var docSet))
            {
                return new HashSet<int>(docSet);
            }
            return new HashSet<int>();
        }

        public HashSet<int> SearchMultiple(params string[] words)
        {
            if (words == null || words.Length == 0) return new HashSet<int>();

            HashSet<int> resultSet = null;

            foreach (var word in words)
            {
                var matches = Search(word);
                if (resultSet == null)
                {
                    resultSet = matches;
                }
                else
                {
                    resultSet.IntersectWith(matches); // Знаходимо документи, які містять УСІ слова
                }
            }

            return resultSet ?? new HashSet<int>();
        }
    }

    // =========================================================================
    // Головна точка входу Program
    // =========================================================================
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Самостійна робота №9: Вивчення Dictionary та HashSet у .NET ===\n");

            // 1. Демонстрація FileMetadataCache
            Console.WriteLine("--- Сценарій 1: Кеш метаданих (Dictionary) ---");
            var cache = new FileMetadataCache();
            cache.Add(new FileMetadata { FileName = "document.pdf", Size = 1048576, LastModified = DateTime.Now, Hash = "A1B2C3D4" });
            cache.Add(new FileMetadata { FileName = "image.png", Size = 512000, LastModified = DateTime.Now, Hash = "E5F6G7H8" });

            Console.WriteLine($"Отримання файлу 'DOCUMENT.PDF' (Case-Insensitive): {cache.Get("DOCUMENT.PDF")}");
            Console.WriteLine($"Чи існує 'image.png'? {cache.Contains("image.png")}\n");

            // 2. Демонстрація TagManager
            Console.WriteLine("--- Сценарій 2: Унікальні теги (HashSet) ---");
            var tagManager = new TagManager();
            tagManager.AddTag("art1", "C#");
            tagManager.AddTag("art1", ".NET");
            tagManager.AddTag("art1", "ООП");

            tagManager.AddTag("art2", ".NET");
            tagManager.AddTag("art2", "LINQ");
            tagManager.AddTag("art2", "C#");

            Console.WriteLine($"Усі унікальні теги в блозі: {string.Join(", ", tagManager.GetAllUniqueTags())}");
            Console.WriteLine($"Спільні теги для статей 'art1' та 'art2': {string.Join(", ", tagManager.FindCommonTags("art1", "art2"))}\n");

            // 3. Демонстрація SearchIndex
            Console.WriteLine("--- Сценарій 3: Індекс для пошуку (Dictionary + HashSet) ---");
            var index = new SearchIndex();
            index.AddDocument(101, "ООП в C# забезпечує інкапсуляцію та поліморфізм");
            index.AddDocument(102, "Колекції Dictionary та HashSet у мові C# гарантують швидкість O(1)");
            index.AddDocument(103, "LINQ та C# полегшують роботу з масивами");

            Console.WriteLine("Пошук слова 'C#': Документи -> " + string.Join(", ", index.Search("C#")));
            Console.WriteLine("Пошук слів 'C#' ТА 'колекції': Документи -> " + string.Join(", ", index.SearchMultiple("C#", "колекції")));
        }
    }
}
