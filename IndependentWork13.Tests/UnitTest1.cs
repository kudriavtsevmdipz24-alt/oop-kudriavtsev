using System.Collections.Generic;
using IndependentWork13;
using Xunit;

namespace IndependentWork13.Tests
{
    public class CacheTests
    {
        [Fact]
        public void AddAndGet_ValidKey_ReturnsValue()
        {
            var cache = new Cache<int, string>(3);
            cache.Add(1, "Value1");
            Assert.Equal("Value1", cache.Get(1));
        }

        [Fact]
        public void Count_ReturnsCurrentItemsCount()
        {
            var cache = new Cache<string, int>(5);
            cache.Add("a", 10);
            cache.Add("b", 20);
            Assert.Equal(2, cache.Count);
        }

        [Fact]
        public void ContainsKey_ReturnsExpectedResult()
        {
            var cache = new Cache<string, string>(2);
            cache.Add("exists", "yes");

            Assert.True(cache.ContainsKey("exists"));
            Assert.False(cache.ContainsKey("missing"));
        }

        [Fact]
        public void Get_MissingKey_ThrowsKeyNotFoundException()
        {
            var cache = new Cache<int, string>(2);
            Assert.Throws<KeyNotFoundException>(() => cache.Get(404));
        }

        [Fact]
        public void Add_WhenFull_EvictsOldestElement_FIFO()
        {
            var cache = new Cache<int, string>(2);
            cache.Add(1, "First");
            cache.Add(2, "Second");
            cache.Add(3, "Third"); // Має витіснити 1

            Assert.Equal(2, cache.Count);
            Assert.False(cache.ContainsKey(1));
            Assert.True(cache.ContainsKey(2));
            Assert.True(cache.ContainsKey(3));
        }

        [Fact]
        public void Add_ExistingKey_UpdatesValueWithoutEviction()
        {
            var cache = new Cache<string, string>(2);
            cache.Add("k1", "Old");
            cache.Add("k2", "Second");
            cache.Add("k1", "New"); // Оновлення існуючого ключа не витісняє елементи

            Assert.Equal(2, cache.Count);
            Assert.Equal("New", cache.Get("k1"));
            Assert.True(cache.ContainsKey("k2"));
        }
    }
}