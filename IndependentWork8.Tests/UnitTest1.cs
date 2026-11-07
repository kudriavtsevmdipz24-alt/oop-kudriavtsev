using System.Collections.Generic;
using IndependentWork8;
using Xunit;

namespace IndependentWork8.Tests
{
    public class CacheTests
    {
        [Fact]
        public void AddAndGet_ValidKey_ReturnsCorrectValue()
        {
            // Arrange
            var cache = new Cache<int, string>(3);

            // Act
            cache.Add(1, "Тестове значення");
            var result = cache.Get(1);

            // Assert
            Assert.Equal("Тестове значення", result);
        }

        [Fact]
        public void Count_ReflectsNumberOfItemsInCache()
        {
            // Arrange
            var cache = new Cache<string, int>(5);

            // Act
            cache.Add("A", 100);
            cache.Add("B", 200);

            // Assert
            Assert.Equal(2, cache.Count);
        }

        [Fact]
        public void ContainsKey_ExistingAndNonExistingKeys_ReturnsCorrectBool()
        {
            // Arrange
            var cache = new Cache<string, string>(2);
            cache.Add("key1", "val1");

            // Assert
            Assert.True(cache.ContainsKey("key1"));
            Assert.False(cache.ContainsKey("key2"));
        }

        [Fact]
        public void Get_NonExistingKey_ThrowsKeyNotFoundException()
        {
            // Arrange
            var cache = new Cache<int, double>(3);

            // Act & Assert
            Assert.Throws<KeyNotFoundException>(() => cache.Get(999));
        }

        [Fact]
        public void Add_ExceedingCapacity_EvictsOldestKey_FIFO()
        {
            // Arrange
            var cache = new Cache<int, string>(2); // Ємність = 2

            // Act
            cache.Add(10, "Перший");
            cache.Add(20, "Другий");
            cache.Add(30, "Третій"); // Має видалити 10 (найстаріший)

            // Assert
            Assert.Equal(2, cache.Count);
            Assert.False(cache.ContainsKey(10)); // 10 видалено
            Assert.True(cache.ContainsKey(20));  // 20 залишився
            Assert.True(cache.ContainsKey(30));  // 30 новий
        }

        [Fact]
        public void Add_ExistingKey_UpdatesValueWithoutIncreasingCount()
        {
            // Arrange
            var cache = new Cache<string, string>(3);
            cache.Add("id1", "Старе значення");

            // Act
            cache.Add("id1", "Оновлене значення");

            // Assert
            Assert.Equal(1, cache.Count);
            Assert.Equal("Оновлене значення", cache.Get("id1"));
        }
    }
}